using Sito.QR.Api.Shared.Enum;

namespace Sito.QR.Api.Shared.Dto;

public interface IQrRequest
{
    QrType Type { get; }
}