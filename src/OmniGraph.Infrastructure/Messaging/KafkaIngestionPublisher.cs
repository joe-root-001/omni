using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OmniGraph.Application.Abstractions;
using OmniGraph.Domain.Models;
using OmniGraph.Infrastructure.Options;

namespace OmniGraph.Infrastructure.Messaging;

public sealed class KafkaIngestionPublisher(
    ILogger<KafkaIngestionPublisher> logger,
    IOptions<KafkaOptions> kafkaOptions) : IIngestionPublisher
{
    public Task PublishAsync(IngestionJob job, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        KafkaPublisherLogMessages.PublishingIngestionJob(
            logger,
            job.Id,
            job.ArtifactUri,
            kafkaOptions.Value.RawTopic);

        return Task.CompletedTask;
    }
}

internal static partial class KafkaPublisherLogMessages
{
    [LoggerMessage(
        EventId = 3001,
        Level = LogLevel.Information,
        Message = "Publishing ingestion job {JobId} for {ArtifactUri} to Kafka topic {Topic}")]
    public static partial void PublishingIngestionJob(ILogger logger, string jobId, string artifactUri, string topic);
}
