using QRCoder;
using Sito.QR.Api.Factories.Abstractions;

namespace Sito.QR.Api.Factories;

internal class QrFactory : IQrFactory
{
    public byte[] Create(string payload)
    {
        var qrGenerator = new QRCodeGenerator();
        var qrCodeData = qrGenerator.CreateQrCode(payload, QRCodeGenerator.ECCLevel.Q);
        var qrCode = new PngByteQRCode(qrCodeData);
        return qrCode.GetGraphic(10);
    }
}