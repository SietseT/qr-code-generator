using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Sito.QR.Generators;
using Sito.QR.Generators.Factories;
using Sito.QR.Generators.Factories.Abstractions;
using Sito.QR.Generators.Shared.Dto;

[assembly: FunctionsStartup(typeof(Startup))]

namespace Sito.QR.Generators;

public class Startup : FunctionsStartup
{
    public override void Configure(IFunctionsHostBuilder builder)
    {
        ConfigureGeneral(builder);
        ConfigurePayloadFactories(builder);
    }

    private void ConfigureGeneral(IFunctionsHostBuilder builder)
    {
        builder.Services.AddSingleton<IQrFactory, QrFactory>();
    }
    
    private void ConfigurePayloadFactories(IFunctionsHostBuilder builder)
    {
        builder.Services.AddSingleton<IPayloadFactory<WifiQrRequest>, WifiPayloadFactory>();
    }
}
