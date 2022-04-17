using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Azure.WebJobs.Logging;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Sito.QR.Api.Shared;
using Sito.QR.Api.Shared.Helpers;

namespace Sito.QR.Api.Shared;

public class StartupBase : FunctionsStartup
{
    private IConfiguration? _configuration;

    public override void Configure(IFunctionsHostBuilder builder)
    {
        _configuration = builder.GetContext().Configuration;

        var services = builder.Services;
        
        services.AddAzureClients(builder =>
        {
            builder.AddServiceBusClient(_configuration.GetConnectionString(Connections.ServiceBus));
        });
        
        services.AddLogging();
        services.AddSingleton<ILogger>(provider => provider.GetService<ILoggerFactory>()
            .CreateLogger(LogCategories.CreateFunctionUserCategory("Common")));
    }
}
