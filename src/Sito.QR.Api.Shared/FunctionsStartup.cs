using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Azure.WebJobs.Logging;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Sito.QR.Api.Shared.Helpers;

namespace Sito.QR.Api.Shared;

public class StartupBase : FunctionsStartup
{
    private IConfiguration? _configuration;

    public override void ConfigureAppConfiguration(IFunctionsConfigurationBuilder builder)
    {
        var context = builder.GetContext();

        builder.ConfigurationBuilder
            .AddJsonFile(Path.Combine(context.ApplicationRootPath, "settings.json"), optional: true, reloadOnChange: false)
            .AddJsonFile(Path.Combine(context.ApplicationRootPath, "local.settings.json"), optional: true, reloadOnChange: false)
            .AddEnvironmentVariables();
        
        base.ConfigureAppConfiguration(builder);
    }

    public override void Configure(IFunctionsHostBuilder builder)
    {
        _configuration = builder.GetContext().Configuration;

        var services = builder.Services;
        
        services.AddAzureClients(factoryBuilder =>
        {
            factoryBuilder.AddServiceBusClient(_configuration.GetConnectionString(Connections.ServiceBus));
        });
        
        services.AddLogging();
        services.AddSingleton(provider => provider.GetRequiredService<ILoggerFactory>()
            .CreateLogger(LogCategories.CreateFunctionUserCategory("Common")));
    }
}
