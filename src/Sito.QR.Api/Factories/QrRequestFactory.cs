using System;
using Newtonsoft.Json;
using Sito.QR.Api.Factories.Abstractions;
using Sito.QR.Api.Shared.Dto;
using Sito.QR.Api.Shared.Enum;

namespace Sito.QR.Api.Factories;

internal class QrRequestFactory : IQrRequestFactory
{
    public IQrRequest? Create(string json)
    {
        var typeRequest = JsonConvert.DeserializeObject<QrRequestBase>(json);
        if(typeRequest == null)
            throw new ArgumentException("Invalid json", nameof(json));

        if (typeRequest.Type == QrType.Wifi)
            return JsonConvert.DeserializeObject<WifiQrRequest>(json);

        return null;
    }
}