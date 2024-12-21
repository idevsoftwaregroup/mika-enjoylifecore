using Microsoft.Extensions.Options;
using RabbitMQ.Client;

namespace Messaging.Infrastructure.Services.MessageQueue;


public class RabbitConnectionProvider : IDisposable
{
    private readonly MessageQueueSettings _settings;
    private readonly IConnection _connection;
    public RabbitConnectionProvider(IOptions<MessageQueueSettings> settingsOptions)
    {
        _settings = settingsOptions.Value;

        ConnectionFactory factory = new();
        factory.Uri = new Uri(_settings.UriString);
        factory.ClientProvidedName = _settings.ClientProvidedName;

        //_connection = factory.CreateConnection();
    }


    public IModel GetChannel()
    {
        IModel channel = _connection.CreateModel();

        channel.ExchangeDeclare(_settings.ExchangeName, ExchangeType.Direct);
        channel.QueueDeclare(_settings.QueueName, false, false, false);
        channel.QueueBind(_settings.QueueName, _settings.ExchangeName, _settings.RoutingKey, null);

        return channel;
    }
    public void Dispose()
    {
        _connection.Close();
    }


}
