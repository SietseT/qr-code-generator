using Sito.QR.Api.Shared.Enum;

namespace Sito.QR.Api.Shared.Dto;

public abstract class QrRequestBase : IQrRequest
{
    protected QrRequestBase(QrType type)
    {
        Type = type;
    }
    
    public QrType Type { get; }
}