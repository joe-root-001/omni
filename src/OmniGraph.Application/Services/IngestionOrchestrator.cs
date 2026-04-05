using OmniGraph.Application.Abstractions;
using OmniGraph.Application.Commands;
using OmniGraph.Domain.Enums;
using OmniGraph.Domain.Models;

namespace OmniGraph.Application.Services;

public sealed class IngestionOrchestrator(IIngestionPublisher ingestionPublisher)
{
    public async Task<IngestionJob> SubmitAsync(SubmitIngestionCommand command, CancellationToken cancellationToken)
    {
        var job = new IngestionJob(
            Id: $"job-{Guid.NewGuid():N}",
            CorrelationId: string.IsNullOrWhiteSpace(command.CorrelationId) ? Guid.NewGuid().ToString("N") : command.CorrelationId!,
            RequestedBy: command.RequestedBy,
            ArtifactKind: command.ArtifactKind,
            ArtifactUri: command.ArtifactUri,
            SubmittedAtUtc: DateTimeOffset.UtcNow,
            Status: IngestionStatus.Queued,
            Metadata: command.Metadata ?? new Dictionary<string, string>());

        await ingestionPublisher.PublishAsync(job, cancellationToken);
        return job;
    }
}
