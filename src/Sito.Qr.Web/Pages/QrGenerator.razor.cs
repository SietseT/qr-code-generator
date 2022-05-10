using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.SignalR.Client;
using Sito.QR.Api.Shared.Constants;
using Sito.QR.Api.Shared.Dto;

namespace Sito.Qr.Web.Pages;

public partial class QrGenerator
{
    private HubConnection? _hubConnection;
    private string _status = "Connecting...";
    private string? _generatedQrCodeUrl;
    
    protected override async Task OnInitializedAsync()
    {
        await StartHubConnection();
    }
    
    private async Task StartHubConnection()
    {
        _hubConnection = new HubConnectionBuilder()
            .WithUrl("http://localhost:7073")
            .ConfigureLogging(builder =>
            {
                builder.SetMinimumLevel(LogLevel.Trace);
            })
            .Build();
        
        await _hubConnection.StartAsync();
        if (_hubConnection.State == HubConnectionState.Connected)
        {
            _status = "Connected";
            _hubConnection.On<GeneratedQr>(QrHubMethods.QrCodeGenerated, data =>
            {
                _generatedQrCodeUrl = data.QrCodeUrl;
            });
        }
        else
        {
            _status = "Failed to set up connection";
        }
    }

    private async Task OnSubmitNote(MouseEventArgs mouseEventArgs)
    {
        if (_hubConnection == null)
            return;
        
        await _hubConnection.SendAsync("sendqrrequest", 
            new WifiQrRequest("test", "test"));
        
        _status = "Request sent...";
    }
}