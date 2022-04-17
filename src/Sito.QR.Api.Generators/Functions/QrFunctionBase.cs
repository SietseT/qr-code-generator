using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Sito.QR.Api.Generators.Factories.Abstractions;
using Sito.QR.Api.Shared.Dto;

namespace Sito.QR.Api.Generators.Functions;

public abstract class QrFunctionBase<TDto> where TDto : IQrRequest
{
    private readonly IPayloadFactory<TDto> _payloadFactory;
    private readonly IQrFactory _qrFactory;

    protected QrFunctionBase(IPayloadFactory<TDto> payloadFactory, IQrFactory qrFactory)
    {
        _payloadFactory = payloadFactory;
        _qrFactory = qrFactory;
    }

    protected async Task<IActionResult> RunHttpAsyncInternal(HttpRequest httpRequest)
    {
        var requestBody = await httpRequest.ReadAsStringAsync();
        
        var dto = JsonConvert.DeserializeObject<TDto>(requestBody);
        if(dto == null)
            return new BadRequestResult();

        var payload = _payloadFactory.CreatePayload(dto);
        var qrData = _qrFactory.Create(payload);

        return new FileContentResult(qrData, "image/png");
    }

    protected async Task ServiceBusTriggerAsync(string queueItem, Stream outputBlob, ILogger logger)
    {
        var dto = JsonConvert.DeserializeObject<TDto>(queueItem);
        if (dto == null)
        {
            logger.LogWarning("Could not deserialize queue item to {Type}. Queue item: {QueueItem}", typeof(TDto), queueItem);
            return;
        }

        var payload = _payloadFactory.CreatePayload(dto);
        var qrData = _qrFactory.Create(payload);

        await outputBlob.WriteAsync(new ReadOnlyMemory<byte>(qrData));
    }
}