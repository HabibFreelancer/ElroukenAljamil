using System.Text;
using System.Text.Json;
using ElroukenAljamil.BuildingBlocks.EventBus.Abstractions;
using ElroukenAljamil.BuildingBlocks.EventBus.Events;
using ElroukenAljamil.BuildingBlocks.Events.Abstractions;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;


namespace ElroukenAljamil.BuildingBlocks.EventBus.RabbitMQ
{
    /// <summary>
    /// Implémentation du bus d'événements avec RabbitMQ.
    /// </summary>
    public class RabbitMQEventBus : IEventBus, IDisposable
    {
        private readonly IConnection _connection;
        private readonly IModel _channel;
        private readonly ILogger<RabbitMQEventBus> _logger;
        private const string ExchangeName = "marketplace_events";


        public RabbitMQEventBus(IConnection connection, ILogger<RabbitMQEventBus> logger)
        {
            _connection = connection;
            _logger = logger;
            _channel = _connection.CreateModel();
            _channel.ExchangeDeclare(ExchangeName, ExchangeType.Topic, durable: true);
        }


        public Task PublishAsync<T>(T @event, CancellationToken cancellationToken = default) where T : IntegrationEvent
        {
            var routingKey = typeof(T).Name;
            var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(@event));


            var properties = _channel.CreateBasicProperties();
            properties.Persistent = true;
            properties.MessageId = @event.EventId.ToString();
            properties.Timestamp = new AmqpTimestamp(DateTimeOffset.UtcNow.ToUnixTimeSeconds());


            _channel.BasicPublish(
                exchange: ExchangeName,
                routingKey: routingKey,
                basicProperties: properties,
                body: body);


            _logger.LogInformation("Événement {EventType} publié avec ID {EventId}", routingKey, @event.EventId);


            return Task.CompletedTask;
        }


        public void Subscribe<T, THandler>()
            where T : IntegrationEvent
            where THandler : IIntegrationEventHandler<T>
        {
            var queueName = $"{typeof(THandler).Name}_{typeof(T).Name}";
            var routingKey = typeof(T).Name;


            _channel.QueueDeclare(queueName, durable: true, exclusive: false, autoDelete: false);
            _channel.QueueBind(queueName, ExchangeName, routingKey);


            _logger.LogInformation("Abonnement {Handler} -> {Event}", typeof(THandler).Name, routingKey);
        }


        public void Dispose()
        {
            _channel?.Dispose();
            _connection?.Dispose();
        }
    }

}
