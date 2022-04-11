using Sito.QR.Generators.Shared.Enum;

namespace Sito.QR.Generators.Shared.Dto;

public abstract class QrRequestBase : IQrRequest
{
    public abstract QrType Type { get; }
}