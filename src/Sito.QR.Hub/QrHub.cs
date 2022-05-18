using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Json;
using System.Text;
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
using Newtonsoft.Json.Linq;
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
    
    [FunctionName(QrHubMethods.SendQrRequest)]
    public async Task SendQrRequest([SignalRTrigger]InvocationContext invocationContext, ILogger logger)
    {
        try
        {
            if(invocationContext.Arguments[0] is not JObject jsonBody)
                throw new Exception("Invalid request body");

            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Post, _hubOptions.QrApiUrl);
            httpRequestMessage.Headers.Add(SignalRHttpHeaders.SignalRConnectionReferer, invocationContext.ConnectionId);
            httpRequestMessage.Content = new StringContent(jsonBody.ToString(), Encoding.UTF8, "application/json");
            await _httpClient.SendAsync(httpRequestMessage);
            
            await Clients.Client(invocationContext.ConnectionId).SendAsync(QrHubMethods.QrRequestSent);
        }
        catch (Exception e)
        {
            logger.LogError(e, "Error sending QR request: {Message}", e.Message);
            throw;
        }
    }
    
    [FunctionName("QrBlobTrigger")]
    public async Task RunAsync([BlobTrigger("generated-qr/{name}", Connection = "StorageAccount")] Stream file,
        string name, ILogger log)
    {
        log.LogInformation("C# Blob trigger function Processed blob\n Name:{Name} \n Size: {Length} Bytes", name, file.Length);

        using var memoryStream = new MemoryStream();
        await file.CopyToAsync(memoryStream);
            
        var base64 = Convert.ToBase64String(memoryStream.ToArray());
        var dataUrl = "data:image/png;base64," + base64;

        var generatedQr = new GeneratedQr(dataUrl);
 
        await Clients.All.SendAsync(QrHubMethods.QrCodeGenerated, generatedQr);
    }
}