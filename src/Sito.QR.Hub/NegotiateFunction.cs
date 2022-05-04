using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Extensions.SignalRService;
using Microsoft.Extensions.Logging;

namespace Sito.QR.Hub;

public static class NegotiateFunction
{
    [FunctionName("negotiate")]
    public static SignalRConnectionInfo Negotiate([HttpTrigger(AuthorizationLevel.Anonymous)] HttpRequest req,
        [SignalRConnectionInfo(HubName = "qr")] SignalRConnectionInfo connectionInfo, ILogger log)
    {
        log.LogInformation("Url: {Url}. Access token: {AccessToken}",connectionInfo.Url, connectionInfo.AccessToken);
        return connectionInfo;
    }
}