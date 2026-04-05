using HotChocolate;
using OmniGraph.Api.Mappings;
using OmniGraph.Application.Commands;
using OmniGraph.Application.Services;
using OmniGraph.Domain.Enums;

namespace OmniGraph.Api.GraphQL;

public sealed class GraphMutation
{
    public async Task<OmniGraph.Contracts.Ingestion.IngestionJobResponse> SubmitIngestionAsync(
        SubmitIngestionInput input,
        [Service] IngestionOrchestrator orchestrator,
        CancellationToken cancellationToken)
    {
        var job = await orchestrator.SubmitAsync(
            new SubmitIngestionCommand(
                input.ArtifactUri,
                input.ArtifactKind,
                input.RequestedBy,
                input.CorrelationId,
                input.Metadata?.ToDictionary(entry => entry.Key, entry => entry.Value)),
            cancellationToken);

        return job.ToResponse();
    }
}

public sealed record SubmitIngestionInput(
    string ArtifactUri,
    ArtifactKind ArtifactKind,
    string RequestedBy,
    string? CorrelationId,
    IReadOnlyCollection<MetadataEntryInput>? Metadata);

public sealed record MetadataEntryInput(
    string Key,
    string Value);
