using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Azure.WebJobs.Logging;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Sito.QR.Api;
using Sito.QR.Api.Factories;
using Sito.QR.Api.Factories.Abstractions;
using Sito.QR.Api.Helpers;
using Sito.QR.Api.Repositories;
using Sito.QR.Api.Repositories.Abstractions;
using Sito.QR.Generators.Shared.Dto;

[assembly: FunctionsStartup(typeof(Startup))]

namespace Sito.QR.Api;

public class Startup : FunctionsStartup
{
    private IConfiguration? _configuration;

    public override void Configure(IFunctionsHostBuilder builder)
    {
        _configuration = builder.GetContext().Configuration;
        
        ConfigureAzureServices(builder.Services);
        ConfigureGeneral(builder.Services);
        ConfigurePayloadFactories(builder.Services);
        ConfigureRequestFactories(builder.Services);
        ConfigureRepositories(builder.Services);
    }

    private void ConfigureAzureServices(IServiceCollection services)
    {
        services.AddAzureClients(builder =>
        {
            builder.AddServiceBusClient(_configuration.GetConnectionString(Connections.ServiceBus));
        });
    }

    private static void ConfigureGeneral(IServiceCollection services)
    {
        services.AddLogging();
        services.AddSingleton<ILogger>(provider => provider.GetService<ILoggerFactory>()
            .CreateLogger(LogCategories.CreateFunctionUserCategory("Common")));
        
        services.AddSingleton<IQrFactory, QrFactory>();
    }
    
    private static void ConfigurePayloadFactories(IServiceCollection services)
    {
        services.AddSingleton<IPayloadFactory<WifiQrRequest>, WifiPayloadFactory>();
    }

    private void ConfigureRequestFactories(IServiceCollection services)
    {
        services.AddSingleton<IQrRequestFactory, QrRequestFactory>();
    }

    private static void ConfigureRepositories(IServiceCollection services)
    {
        services.AddSingleton<IServiceBusRepository, AzureServiceBusRepository>();
    }
}
