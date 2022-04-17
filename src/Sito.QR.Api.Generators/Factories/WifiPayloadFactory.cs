using QRCoder;
using Sito.QR.Api.Shared.Dto;

namespace Sito.QR.Api.Generators.Factories;

internal class WifiPayloadFactory : PayloadFactoryBase<WifiQrRequest>
{
    protected override PayloadGenerator.Payload BuildPayload(WifiQrRequest dto)
    {
        return new PayloadGenerator.WiFi(dto.Ssid, dto.Password, PayloadGenerator.WiFi.Authentication.WPA);
    }
}