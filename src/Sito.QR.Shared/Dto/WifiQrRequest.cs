using Sito.QR.Api.Shared.Enum;

namespace Sito.QR.Api.Shared.Dto;

public class WifiQrRequest : QrRequestBase
{
    public string Ssid { get; }
    public string Password { get; }

    public WifiQrRequest(string ssid, string password) : base(QrType.Wifi)
    {
        Ssid = ssid;
        Password = password;
    }
}