using QRCoder;
using Sito.QR.Generators.Shared.Dto;

namespace Sito.QR.Api.Factories;

internal class WifiPayloadFactory : PayloadFactoryBase<WifiQrRequest>
{
    protected override PayloadGenerator.Payload BuildPayload(WifiQrRequest dto)
    {
        return new PayloadGenerator.WiFi(dto.Ssid, dto.Password, PayloadGenerator.WiFi.Authentication.WPA);
    }
}