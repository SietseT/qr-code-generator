using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Sito.QR.Generators.Factories.Abstractions;
using Sito.QR.Generators.Shared.Dto;

namespace Sito.QR.Generators.Functions;

public class WifiQrGenerator : QrFunctionBase<WifiQrRequest>
{
    public WifiQrGenerator(IPayloadFactory<WifiQrRequest> payloadFactory,  IQrFactory qrFactory) : base(payloadFactory, qrFactory)
    {
    }
    
    [FunctionName(nameof(WifiQrGenerator))]
    public async Task<IActionResult> RunAsync([HttpTrigger(AuthorizationLevel.Function, "post", Route = "qr/wifi")] HttpRequest req, ILogger log)
    {
        return await RunAsyncInternal(req);
    }
}