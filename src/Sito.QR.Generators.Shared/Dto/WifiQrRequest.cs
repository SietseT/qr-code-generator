using Sito.QR.Generators.Shared.Enum;

namespace Sito.QR.Generators.Shared.Dto;

public class WifiQrRequest : QrRequestBase
{
    public string Ssid { get; }
    public string Password { get; }

    public WifiQrRequest(string ssid, string password)
    {
        Ssid = ssid;
        Password = password;
    }

    public override QrType Type => QrType.Wifi;
}