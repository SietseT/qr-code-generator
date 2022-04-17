namespace Sito.QR.Api.Factories.Abstractions;

public interface IQrFactory
{
    byte[] Create(string payload);
}