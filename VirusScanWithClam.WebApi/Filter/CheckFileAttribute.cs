using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using nClam;

namespace VirusScanWithClam.WebApi.Filter
{
    public class CheckFileAttribute : Attribute, IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            IConfiguration configuration = context.HttpContext.RequestServices.GetRequiredService<IConfiguration>();
            ILogger<CheckFileAttribute> logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<CheckFileAttribute>>();
            var formFiles = context.HttpContext.Request.Form.Files;

            foreach (var formFile in formFiles)
            {
                await using var ms = new MemoryStream();
                await formFile.OpenReadStream().CopyToAsync(ms);
                byte[] fileBytes = ms.ToArray();
                var clam = new ClamClient(configuration["ClamAVServer:URL"], Convert.ToInt32(configuration["ClamAVServer:Port"]));

                bool isClamAVAvaible = await clam.TryPingAsync(); //ClamAV programına erişip erişemediğimizi kontrol ettiğimiz kod parçacığı.
                if (isClamAVAvaible)
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