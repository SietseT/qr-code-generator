using System.IO;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Azure.WebJobs.Logging;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Sito.QR.Hub;

[assembly: FunctionsStartup(typeof(Startup))]

namespace Sito.QR.Hub;

public class Startup : FunctionsStartup
{

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
        var services = builder.Services;

        services.AddLogging();
        services.AddSingleton(provider => provider.GetRequiredService<ILoggerFactory>()
            .CreateLogger(LogCategories.CreateFunctionUserCategory("Common")));

        services.AddSignalR();
        
        services.AddOptions<HubOptions>()
            .Configure<IConfiguration>((settings, config) =>
            {
                config.GetSection("Values").Bind(settings);
            });
    }
}
