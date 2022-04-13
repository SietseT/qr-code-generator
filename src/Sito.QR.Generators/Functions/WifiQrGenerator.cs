using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Sito.QR.Generators.Factories.Abstractions;
using Sito.QR.Generators.Shared.Dto;

namespace Sito.QR.Generators.Functions;

public class WifiQrGenerator : QrFunctionBase<WifiQrRequest>
{
    public WifiQrGenerator(IPayloadFactory<WifiQrRequest> payloadFactory,  IQrFactory qrFactory) : base(payloadFactory, qrFactory)
    {
    }
    
    [FunctionName(nameof(WifiQrGenerator) + "Http")]
    public async Task<IActionResult> RunAsync([HttpTrigger(AuthorizationLevel.Function, "post", Route = "qr/wifi")] HttpRequest req, ILogger log)
    {
        return await RunHttpAsyncInternal(req);
    }
    
    [FunctionName(nameof(WifiQrGenerator) + "ServiceBus")]
    [StorageAccount("StorageAccount")]
    public async Task Run(
        [ServiceBusTrigger("wifi", Connection = "ServiceBus")] string queueItem,
        [Blob("wifi/{rand-guid}.png", FileAccess.Write)] Stream qrBlob,
        ILogger logger)
    {
        await ServiceBusTriggerAsync(queueItem, qrBlob, logger);
    }
}