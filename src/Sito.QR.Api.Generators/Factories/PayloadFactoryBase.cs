using QRCoder;
using Sito.QR.Api.Generators.Factories.Abstractions;
using Sito.QR.Api.Shared.Dto;

namespace Sito.QR.Api.Generators.Factories;

internal abstract class PayloadFactoryBase<TDto> : IPayloadFactory<TDto> where TDto : IQrRequest
{
    protected abstract PayloadGenerator.Payload BuildPayload(TDto dto);
    public string CreatePayload(TDto dto)
    {
        var payLoad = BuildPayload(dto);
        return payLoad.ToString();
    }
}