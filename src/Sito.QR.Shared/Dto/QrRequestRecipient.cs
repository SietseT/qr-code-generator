namespace Sito.QR.Api.Shared.Dto;

public class QrRequestRecipient<T> where T : IQrRequest
{
    public QrRequestRecipient(T request, string requestorId)
    {
        Request = request;
        RequestorId = requestorId;
    }

    public T Request { get; }
    public string RequestorId { get; }
}