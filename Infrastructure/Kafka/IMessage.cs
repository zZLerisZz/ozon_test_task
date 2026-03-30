namespace Infrastructure.Kafka;

public interface IMessage {
    Task PublishAsync<T>(T message);
}

