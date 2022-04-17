using Sito.QR.Api.Shared.Dto;

namespace Sito.QR.Api.Factories.Abstractions;

public interface IQrRequestFactory
{
    IQrRequest? Create(string json);
}