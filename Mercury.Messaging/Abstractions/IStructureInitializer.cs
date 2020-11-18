using RabbitMQ.Client;

namespace Mercury.Messaging.Abstractions
{
    public interface IStructureInitializer
    {
        void Initialize(IModel channel);
    }
}