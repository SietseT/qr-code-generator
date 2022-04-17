using QRCoder;
using Sito.QR.Api.Factories.Abstractions;
using Sito.QR.Generators.Shared.Dto;

namespace Sito.QR.Api.Factories;

internal abstract class PayloadFactoryBase<TDto> : IPayloadFactory<TDto> where TDto : IQrRequest
{
    protected abstract PayloadGenerator.Payload BuildPayload(TDto dto);
    public string CreatePayload(TDto dto)
    {
        var payLoad = BuildPayload(dto);
        return payLoad.ToString();
    }
}