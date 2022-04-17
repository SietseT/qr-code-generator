using Sito.QR.Generators.Shared.Enum;

namespace Sito.QR.Generators.Shared.Dto;

public class QrRequestBase : IQrRequest
{
    public QrRequestBase(QrType type)
    {
        Type = type;
    }
    
    public QrType Type { get; }
}