using Sito.QR.Generators.Shared.Enum;

namespace Sito.QR.Generators.Shared.Dto;

public interface IQrRequest
{
    QrType Type { get; }
}