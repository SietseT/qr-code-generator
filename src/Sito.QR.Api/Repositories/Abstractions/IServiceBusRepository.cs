using System.Threading.Tasks;

namespace Sito.QR.Api.Repositories.Abstractions;

public interface IServiceBusRepository
{
    Task SendMessageAsync<T>(string queueName, T messageObject);
}