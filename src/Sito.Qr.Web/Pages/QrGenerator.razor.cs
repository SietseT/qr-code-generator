using Microsoft.AspNetCore.SignalR.Client;

namespace Sito.Qr.Web.Pages;

public partial class QrGenerator
{
    private HubConnection? _hubConnection;
    private string _connectionStatus = "Connecting...";
    protected override async Task OnInitializedAsync()
    {
        await StartHubConnection();
    }
    
    private async Task StartHubConnection()
    {
        _hubConnection = new HubConnectionBuilder()
            .WithUrl("http://localhost:7073/api")
            .Build();
        
        await _hubConnection.StartAsync();
        if (_hubConnection.State == HubConnectionState.Connected)
            _connectionStatus = "Connected";
    }
    
}