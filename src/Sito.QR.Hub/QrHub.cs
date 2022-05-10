using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Extensions.SignalRService;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Sito.QR.Api.Shared.Constants;
using Sito.QR.Api.Shared.Dto;

namespace Sito.QR.Hub;

public class QrHub : ServerlessHub
{
    private readonly HubOptions _hubOptions;
    private readonly HttpClient _httpClient;

    public QrHub(IOptions<HubOptions> hubOptions, IHttpClientFactory httpClientFactory)
    {
        _hubOptions = hubOptions.Value;
        _httpClient = httpClientFactory.CreateClient();
    }
    
    [FunctionName("negotiate")]
    public SignalRConnectionInfo Negotiate([HttpTrigger(AuthorizationLevel.Anonymous, Route = "negotiate")]HttpRequest req)
    {
        return Negotiate();
    }
    
    [FunctionName(nameof(OnConnected))]
    public void OnConnected([SignalRTrigger]InvocationContext invocationContext, ILogger logger)
    {
        logger.LogInformation("{ConnectionId} has connected", invocationContext.ConnectionId);
    }
    
    [FunctionName("sendqrrequest")]
    public async Task SendQrRequest([SignalRTrigger]InvocationContext invocationContext, ILogger logger)
    {
        try
        {
            if(invocationContext.Arguments[0] is not string jsonBody)
                throw new Exception("Invalid request body");
            
            

            // logger.LogInformation("Message received: {Message}", message);
            await Clients.All.SendAsync("test", "test");
        }
        catch (Exception e)
        {
            var t = true;
        }
    }
}