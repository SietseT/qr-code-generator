namespace Sito.QR.Api.Shared.Dto;

public class GeneratedQr
{
    public GeneratedQr(string qrCodeUrl)
    {
        QrCodeUrl = qrCodeUrl;
    }

    public string QrCodeUrl { get; }
}