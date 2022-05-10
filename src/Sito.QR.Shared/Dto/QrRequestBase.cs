using Sito.QR.Api.Shared.Enum;

namespace Sito.QR.Api.Shared.Dto;

/// <summary>
/// Both class and constructor need to be public for serialization to work
/// </summary>
public class QrRequestBase : IQrRequest
{
    // ReSharper disable once MemberCanBeProtected.Global
    public QrRequestBase(QrType type)
    {
        Type = type;
    }
    
    public QrType Type { get; }
}