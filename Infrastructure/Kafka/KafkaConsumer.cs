using Confluent.Kafka;
using OzonTestTask.Domain;
using System.Text.Json;

namespace Infrastructure.Kafka;
    
/// <summary>
/// Kafka консюмер, который использует worker для получения данных через шину
/// </summary>
public class KafkaConsumer {
    private readonly IConsumer<string, string> _consumer;

    public KafkaConsumer() {
        var config = new ConsumerConfig {
            BootstrapServers = "kafka:9092",
            GroupId = "report-worker",
            AutoOffsetReset = AutoOffsetReset.Earliest,
            AllowAutoCreateTopics = true,
            EnableAutoCommit = false
        };
        _consumer = new ConsumerBuilder<string, string>(config).Build();
    }

    public async Task ConsumeAsync(Func<ReportRequestEvent, Task> handler, CancellationToken token) {
        while(!token.IsCancellationRequested) {
            try {
                _consumer.Subscribe("report-requests");
                while(!token.IsCancellationRequested) {
                    var result = _consumer.Consume(token);
                    var message = JsonSerializer.Deserialize<ReportRequestEvent>(result.Message.Value);
                    if(message != null) {
                        await handler(message);
                        _consumer.Commit(result);
                    }
                }
            }
            catch(ConsumeException ex) {
                Console.WriteLine($"Kafka error: {ex.Error.Reason}");
                Console.WriteLine("Retrying in 5 seconds...");
                await Task.Delay(5000, token);
            }
            catch(Exception ex) {
                Console.WriteLine($"Unexpected error: {ex.Message}");
                await Task.Delay(5000, token);
            }
        }
    }
}
