using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.SignalR.Client;
using Sito.QR.Api.Shared.Constants;
using Sito.QR.Api.Shared.Dto;

namespace Sito.Qr.Web.Pages;

public partial class Index
{
    private HubConnection? _hubConnection;
    private string _status = "Connecting...";
    private string? _generatedQrCodeUrl;

    private string _ssid = string.Empty;
    private string _password = string.Empty;
    
    protected override async Task OnInitializedAsync()
    {
        await StartHubConnection();
    }
    
    private async Task StartHubConnection()
    {
        _hubConnection = new HubConnectionBuilder()
            // .WithUrl("http://localhost:7073")
            .WithUrl("https://func-dev-qrgenerator-hub.azurewebsites.net")
            // .WithUrl("https://7c2f-83-82-118-241.eu.ngrok.io")
            .AddJsonProtocol(cfg =>
            {
                var jsonOptions = new System.Text.Json.JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };
                jsonOptions.Converters.Add(new JsonStringEnumConverter());

                cfg.PayloadSerializerOptions = jsonOptions;
            })
            .Build();
        
        _hubConnection.On<GeneratedQr>(QrHubMethods.QrCodeGenerated, data =>
        {
            _generatedQrCodeUrl = data.QrCodeUrl;
            _status = "QR code generated!";
            StateHasChanged(); //Force state change because SignalR + Blazor doesn't seem to do it for us
        });
            
        _hubConnection.On(QrHubMethods.QrRequestSent, () =>
        {
            _status = "QR code is being generated...";
            StateHasChanged(); //Force state change because SignalR + Blazor doesn't seem to do it for us
        });
        
        await _hubConnection.StartAsync();
        if (_hubConnection.State == HubConnectionState.Connected)
            _status = "Connected!";
        else
            _status = "Failed to set up connection";
    }

    private async Task OnSubmitQr(MouseEventArgs mouseEventArgs)
    {
        if (_hubConnection == null)
            return;
        
        await _hubConnection.SendAsync(QrHubMethods.SendQrRequest, 
            new WifiQrRequest(_ssid, _password));
        
        _status = "Request sent...";
    }
}