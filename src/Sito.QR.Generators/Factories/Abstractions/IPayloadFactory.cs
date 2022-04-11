using Sito.QR.Generators.Shared.Dto;

namespace Sito.QR.Generators.Factories.Abstractions;

public interface IPayloadFactory<in TDto> where TDto : IQrRequest
{
    string CreatePayload(TDto dto);
}