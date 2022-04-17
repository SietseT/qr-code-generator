using Sito.QR.Api.Shared.Dto;

namespace Sito.QR.Api.Generators.Factories.Abstractions;

public interface IPayloadFactory<in TDto> where TDto : IQrRequest
{
    string CreatePayload(TDto dto);
}