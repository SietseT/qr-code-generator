namespace Sito.QR.Generators.Factories.Abstractions;

public interface IQrFactory
{
    byte[] Create(string payload);
}