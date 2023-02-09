using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Options;
using nClam;
using VirusScanWithClam.WebApi.Models;

namespace VirusScanWithClam.WebApi.Filter
{
    public class CheckFileAttribute : ActionFilterAttribute
    {
        private readonly ClamAVServer _clamAVServer;
        public CheckFileAttribute(IOptions<ClamAVServer> options)
        {
            _clamAVServer = options.Value;
        }
        public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            ILogger<CheckFileAttribute> logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<CheckFileAttribute>>();
            var formFiles = context.HttpContext.Request.Form.Files;

            foreach (var formFile in formFiles)
            {
                await using var ms = new MemoryStream();
                await formFile.OpenReadStream().CopyToAsync(ms);
                byte[] fileBytes = ms.ToArray();
                var clam = new ClamClient(_clamAVServer.URL, _clamAVServer.Port);

                bool isClamConnect = await clam.TryPingAsync(); //ClamAV programına erişip erişemediğimizi kontrol ettiğimiz kod parçacığı.
                if (isClamConnect)
                {
                    var scanResult = await clam.SendAndScanFileAsync(fileBytes); //Dosyayı gönderip sonucunu scanResult değişkenine aktardığımız kod parçacığı.
                    if (scanResult.Result == ClamScanResults.Clean)
                    {
                        await next();
                    }
                    else
                    {
                        context.Result = new BadRequestObjectResult(scanResult.RawResult);
                    }
                }
                else
                {
                    logger.LogWarning("ClamAv is not installed on this server");
                    await next();
                }
            }
        }
    }
}