using Sito.QR.Api.Shared.Enum;

namespace Sito.QR.Api.Shared.Dto;

public class QrRequestBase : IQrRequest
{
    public QrRequestBase(QrType type)
    {
        Type = type;
    }
    
    public QrType Type { get; }
}