using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;
using EventDrivenArchDemo.Api.Domain.Messaging;

namespace EventDrivenArchDemo.Api.Infra.Messaging
{
    public class RabbitMqMessagePublisher : IMessagePublisher
    {
        private readonly IConfiguration _configuration;

        public RabbitMqMessagePublisher(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task PublishAsync<T>(string exchange, string routingKey, T payload)
        {
            var rabbitConfig = _configuration.GetSection("RabbitMQ");
            var factory = new ConnectionFactory()
            {
                HostName = rabbitConfig["HostName"],
                UserName = rabbitConfig["UserName"],
                Password = rabbitConfig["Password"]
            };

            using var connection = await factory.CreateConnectionAsync();
            using var channel = await connection.CreateChannelAsync();

            var message = JsonSerializer.Serialize(payload);
            var body = Encoding.UTF8.GetBytes(message);

            var props = new BasicProperties();
            props.Persistent = true;

            await channel.BasicPublishAsync(
                exchange: exchange,
                routingKey: routingKey,
                mandatory: false,
                basicProperties: props,
                body: body
            );
        }
    }
}
