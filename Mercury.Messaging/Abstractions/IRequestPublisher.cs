namespace Mercury.Messaging.Abstractions
{
    public interface IRequestPublisher<TRequest>
    {
        void Publish(RequestMessage<TRequest> message);
    }
}