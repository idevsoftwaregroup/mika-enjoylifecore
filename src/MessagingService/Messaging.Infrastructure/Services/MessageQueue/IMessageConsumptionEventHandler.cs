using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Messaging.Infrastructure.Services.MessageQueue
{
    public interface IMessageConsumptionEventHandler
    {
        Task Handle(object? sender, BasicDeliverEventArgs args, IModel channel, CancellationToken cancellationToken);
    }
}