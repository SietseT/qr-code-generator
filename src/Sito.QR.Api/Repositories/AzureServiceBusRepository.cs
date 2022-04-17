using System.Threading.Tasks;
using Azure.Messaging.ServiceBus;
using Newtonsoft.Json;
using Sito.QR.Api.Repositories.Abstractions;

namespace Sito.QR.Api.Repositories;

internal class AzureServiceBusRepository : IServiceBusRepository
{
    private readonly ServiceBusClient _serviceBusClient;

    public AzureServiceBusRepository(ServiceBusClient serviceBusClient)
    {
        _serviceBusClient = serviceBusClient;
    }

    public async Task SendMessageAsync<T>(string queueName, T messageObject)
    {
        var sender = _serviceBusClient.CreateSender(queueName);
        var message = new ServiceBusMessage(JsonConvert.SerializeObject(messageObject));

        await sender.SendMessageAsync(message);
    }
}