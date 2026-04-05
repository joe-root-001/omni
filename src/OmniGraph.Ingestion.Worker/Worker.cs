using Microsoft.Extensions.Options;
using OmniGraph.Infrastructure.Options;

namespace OmniGraph.Ingestion.Worker;

public sealed class Worker(
    ILogger<Worker> logger,
    IOptions<KafkaOptions> kafkaOptions) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        WorkerLogMessages.IngestionWorkerStarted(
            logger,
            kafkaOptions.Value.BootstrapServers,
            kafkaOptions.Value.RawTopic);

        while (!stoppingToken.IsCancellationRequested)
        {
            WorkerLogMessages.WaitingForArtifacts(
                logger,
                kafkaOptions.Value.ExtractedTopic);

            await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
        }
    }
}

internal static partial class WorkerLogMessages
{
    [LoggerMessage(
        EventId = 1001,
        Level = LogLevel.Information,
        Message = "OmniGraph ingestion worker listening on {BootstrapServers} for topic {Topic}")]
    public static partial void IngestionWorkerStarted(ILogger logger, string bootstrapServers, string topic);

    [LoggerMessage(
        EventId = 1002,
        Level = LogLevel.Information,
        Message = "Waiting for raw artifacts. Downstream extracted topic: {ExtractedTopic}")]
    public static partial void WaitingForArtifacts(ILogger logger, string extractedTopic);
}
