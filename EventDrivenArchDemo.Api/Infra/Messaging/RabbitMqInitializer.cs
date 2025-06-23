using RabbitMQ.Client;

namespace EventDrivenArchDemo.Api.Infra.Messaging
{
    public static class RabbitMqInitializer
    {
        public static async Task InitializeAsync(IConfiguration configuration)
        {
            var rabbitConfig = configuration.GetSection("RabbitMQ");
            var factory = new ConnectionFactory()
            {
                HostName = rabbitConfig["HostName"],
                UserName = rabbitConfig["UserName"],
                Password = rabbitConfig["Password"]
            };

            await using var connection = await factory.CreateConnectionAsync();
            await using var channel = await connection.CreateChannelAsync();

            await channel.ExchangeDeclareAsync(
                exchange: "RentCreated",
                type: ExchangeType.Topic,
                durable: true
            );

            //await channel.QueueDeclareAsync(
            //    queue: "RentCreated_LibraryReportsFunction",
            //    durable: true,
            //    exclusive: false,
            //    autoDelete: false,
            //    arguments: null
            //);

            //await channel.QueueBindAsync(
            //    queue: "RentCreated_LibraryReportsFunction",
            //    exchange: "RentCreated",
            //    routingKey: "#"
            //);
        }
    }
}
