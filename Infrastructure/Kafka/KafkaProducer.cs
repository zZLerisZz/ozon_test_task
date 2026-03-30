using Confluent.Kafka;
using System.Text.Json;

namespace Infrastructure.Kafka;

/// <summary>
/// Kafka продьюсер, который используется для передачи данных worker'у
/// </summary>
public class KafkaProducer: IMessage {
    private readonly IProducer<string, string> _producer;

    public KafkaProducer() {
        var config = new ProducerConfig {
            BootstrapServers = "kafka:9092"
        };
        _producer = new ProducerBuilder<string, string>(config).Build();
    }

    public async Task PublishAsync<T>(T message) {
        var topic = "report-requests";
        var json = JsonSerializer.Serialize(message);
        await _producer.ProduceAsync(topic, new Message<string, string> {
            Key = Guid.NewGuid().ToString(),
            Value = json
        });
    }
}
