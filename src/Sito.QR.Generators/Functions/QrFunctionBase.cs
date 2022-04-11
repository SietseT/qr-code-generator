using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Newtonsoft.Json;
using Sito.QR.Generators.Factories.Abstractions;
using Sito.QR.Generators.Shared.Dto;

namespace Sito.QR.Generators.Functions;

public abstract class QrFunctionBase<TDto> where TDto : IQrRequest
{
    private readonly IPayloadFactory<TDto> _payloadFactory;
    private readonly IQrFactory _qrFactory;

    public QrFunctionBase(IPayloadFactory<TDto> payloadFactory, IQrFactory qrFactory)
    {
        _payloadFactory = payloadFactory;
        _qrFactory = qrFactory;
    }
    
    public async Task<IActionResult> RunAsyncInternal(HttpRequest httpRequest)
    {
        var requestBody = await httpRequest.ReadAsStringAsync();
        
        var dto = JsonConvert.DeserializeObject<TDto>(requestBody);
        if(dto == null)
            return new BadRequestResult();

        var payload = _payloadFactory.CreatePayload(dto);
        var qrData = _qrFactory.Create(payload);

        return new FileContentResult(qrData, "image/png");
    }
}