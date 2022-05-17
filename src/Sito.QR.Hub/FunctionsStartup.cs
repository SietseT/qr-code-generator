using System.IO;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Azure.WebJobs.Logging;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Sito.QR.Api.Shared;
using Sito.QR.Hub;

[assembly: FunctionsStartup(typeof(Startup))]

namespace Sito.QR.Hub;

public class Startup : StartupBase
{
    public override void Configure(IFunctionsHostBuilder builder)
    {
        base.Configure(builder);
        
        var services = builder.Services;
        
        services.AddOptions<HubOptions>()
            .Configure<IConfiguration>((options, configuration) =>
            {
                // In Azure, binding of settings doesnt work from environment variable.
                configuration.Bind(options);
            });
    }
}
