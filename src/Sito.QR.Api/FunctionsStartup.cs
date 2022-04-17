using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Azure.WebJobs.Logging;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Sito.QR.Api;
using Sito.QR.Api.Factories;
using Sito.QR.Api.Factories.Abstractions;
using Sito.QR.Api.Repositories;
using Sito.QR.Api.Repositories.Abstractions;
using Sito.QR.Api.Shared;
using Sito.QR.Api.Shared.Dto;
using Sito.QR.Api.Shared.Helpers;

[assembly: FunctionsStartup(typeof(Startup))]

namespace Sito.QR.Api;

public class Startup : StartupBase
{
    public override void Configure(IFunctionsHostBuilder builder)
    {
        base.Configure(builder);
        builder.Services.AddSingleton<IQrRequestFactory, QrRequestFactory>();
        builder.Services.AddSingleton<IServiceBusRepository, AzureServiceBusRepository>();
    }
}
