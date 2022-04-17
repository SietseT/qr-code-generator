using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Sito.QR.Api.Generators;
using Sito.QR.Api.Generators.Factories;
using Sito.QR.Api.Generators.Factories.Abstractions;
using Sito.QR.Api.Shared;
using Sito.QR.Api.Shared.Dto;

[assembly: FunctionsStartup(typeof(Startup))]

namespace Sito.QR.Api.Generators;

public class Startup : StartupBase
{
    public override void Configure(IFunctionsHostBuilder builder)
    {
        ConfigureFactories(builder.Services);
    }

    private static void ConfigureFactories(IServiceCollection services)
    {
        services.AddSingleton<IQrFactory, QrFactory>();
        
        services.AddSingleton<IPayloadFactory<WifiQrRequest>, WifiPayloadFactory>();
        
        
    }
}
