namespace EventDrivenArchDemo.Api.Domain.Messaging
{
    public interface IMessagePublisher
    {
        Task PublishAsync<T>(string exchange, string routingKey, T payload);
    }
}
