namespace Sito.QR.Api.Generators.Factories.Abstractions;

public interface IQrFactory
{
    byte[] Create(string payload);
}