using System;
using System.Threading.Tasks;
using System.Web.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Sito.QR.Api.Factories.Abstractions;
using Sito.QR.Api.Repositories.Abstractions;
using Sito.QR.Api.Shared.Dto;

namespace Sito.QR.Api.Functions;

public class QrApi
{
    private readonly IQrRequestFactory _qrRequestFactory;
    private readonly IServiceBusRepository _serviceBusRepository;
    private readonly ILogger _logger;

    public QrApi(IQrRequestFactory qrRequestFactory, IServiceBusRepository serviceBusRepository, ILogger logger)
    {
        _qrRequestFactory = qrRequestFactory;
        _serviceBusRepository = serviceBusRepository;
        _logger = logger;
    }
    
    [FunctionName(nameof(QrApi))]
    public async Task<IActionResult> RunAsync([HttpTrigger(AuthorizationLevel.Function, "post", Route = "qr")] 
        HttpRequest req, ILogger log)
    {
        //_logger = log;
        
        var jsonContent = await req.ReadAsStringAsync();
        if(string.IsNullOrWhiteSpace(jsonContent))
        {
            return new BadRequestObjectResult("Empty request body");
        }

        var qrRequest = GetQrRequest(jsonContent);
        if (qrRequest == null)
            return new BadRequestResult();

        if (!await EnqueueMessage(qrRequest))
            return new InternalServerErrorResult();

        return new OkResult();
    }

    private IQrRequest? GetQrRequest(string jsonContent)
    {
        try 
        { 
            return _qrRequestFactory.Create(jsonContent);
        }
        catch(Exception ex)
        {
            _logger.LogError(ex, "Error constructing QR request");
            return null;
        }
    }

    private async Task<bool> EnqueueMessage(IQrRequest qrRequest)
    {
        try
        {
            await _serviceBusRepository.SendMessageAsync(qrRequest.Type.ToString().ToLowerInvariant(), qrRequest);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error enqueuing message");
            return false;
        }
    }
}