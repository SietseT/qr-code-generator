using Sito.QR.Generators.Shared.Dto;

namespace Sito.QR.Api.Factories.Abstractions;

public interface IPayloadFactory<in TDto> where TDto : IQrRequest
{
    string CreatePayload(TDto dto);
}