using OmniGraph.Application.Abstractions;
using OmniGraph.Application.Models;
using OmniGraph.Infrastructure.Persistence;

namespace OmniGraph.Infrastructure.Explainability;

public sealed class InMemoryExplainabilityService(InMemoryKnowledgeStore store) : IExplainabilityService
{
    public Task<ExplainabilityResult?> ExplainRelationshipAsync(string relationshipId, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var relationship = store.GetRelationship(relationshipId);
        if (relationship is null)
        {
            return Task.FromResult<ExplainabilityResult?>(null);
        }

        var artifact = store.GetArtifact(relationship.SourceArtifactId);
        if (artifact is null)
        {
            return Task.FromResult<ExplainabilityResult?>(null);
        }

        return Task.FromResult<ExplainabilityResult?>(
            new ExplainabilityResult(
                relationship.Id,
                relationship.Explanation,
                artifact,
                relationship.Evidence));
    }
}
