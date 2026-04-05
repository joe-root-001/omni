using OmniGraph.Application.Abstractions;
using OmniGraph.Application.Models;
using OmniGraph.Domain.Enums;
using OmniGraph.Domain.Models;

namespace OmniGraph.Infrastructure.Persistence;

public sealed class InMemoryGraphRepository(InMemoryKnowledgeStore store) : IGraphRepository
{
    public Task<KnowledgeEntity?> GetEntityAsync(string entityId, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return Task.FromResult(store.GetEntity(entityId));
    }

    public Task<GraphQueryResult> SearchAsync(GraphQueryRequest request, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var entities = store.GetEntities();
        var relationships = store.GetRelationships();
        IEnumerable<KnowledgeEntity> candidates = entities;

        if (request.EntityType is not null)
        {
            candidates = candidates.Where(entity => entity.Type == request.EntityType);
        }

        if (request.MinimumRisk is not null)
        {
            candidates = candidates.Where(entity => entity.RiskLevel >= request.MinimumRisk);
        }

        if (!string.IsNullOrWhiteSpace(request.Text))
        {
            var text = request.Text.Trim();
            candidates = candidates.Where(entity =>
                entity.DisplayName.Contains(text, StringComparison.OrdinalIgnoreCase) ||
                entity.ExternalId.Contains(text, StringComparison.OrdinalIgnoreCase) ||
                entity.Properties.Values.Any(value => value.Contains(text, StringComparison.OrdinalIgnoreCase)));
        }

        var anchorIds = candidates.Select(entity => entity.Id).ToHashSet(StringComparer.OrdinalIgnoreCase);

        if (!string.IsNullOrWhiteSpace(request.RelatedToPolicy))
        {
            var relatedPolicyIds = entities
                .Where(entity => entity.Type == KnowledgeEntityType.Policy &&
                                 (entity.DisplayName.Contains(request.RelatedToPolicy, StringComparison.OrdinalIgnoreCase) ||
                                  entity.ExternalId.Contains(request.RelatedToPolicy, StringComparison.OrdinalIgnoreCase)))
                .Select(entity => entity.Id)
                .ToHashSet(StringComparer.OrdinalIgnoreCase);

            candidates = candidates.Where(entity =>
                relationships.Any(relationship =>
                    (relationship.FromEntityId == entity.Id && relatedPolicyIds.Contains(relationship.ToEntityId)) ||
                    (relationship.ToEntityId == entity.Id && relatedPolicyIds.Contains(relationship.FromEntityId))));

            anchorIds = candidates.Select(entity => entity.Id).Concat(relatedPolicyIds).ToHashSet(StringComparer.OrdinalIgnoreCase);
        }

        anchorIds = anchorIds.Take(request.Limit).ToHashSet(StringComparer.OrdinalIgnoreCase);

        var graphIds = Expand(anchorIds, relationships, request.Depth);
        var graphNodes = entities.Where(entity => graphIds.Contains(entity.Id)).ToArray();
        var graphRelationships = relationships
            .Where(relationship => graphIds.Contains(relationship.FromEntityId) && graphIds.Contains(relationship.ToEntityId))
            .ToArray();

        return Task.FromResult(new GraphQueryResult(graphNodes, graphRelationships, null));
    }

    public Task<GraphQueryResult> GetEntityNeighborhoodAsync(string entityId, int depth, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var entities = store.GetEntities();
        var relationships = store.GetRelationships();
        var graphIds = Expand(new HashSet<string>(StringComparer.OrdinalIgnoreCase) { entityId }, relationships, depth);

        return Task.FromResult(new GraphQueryResult(
            entities.Where(entity => graphIds.Contains(entity.Id)).ToArray(),
            relationships.Where(relationship => graphIds.Contains(relationship.FromEntityId) && graphIds.Contains(relationship.ToEntityId)).ToArray(),
            null));
    }

    public Task<ImpactAnalysisResult> AnalyzeImpactAsync(ImpactAnalysisRequest request, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var relationships = store.GetRelationships();
        var graphIds = Expand(new HashSet<string>(StringComparer.OrdinalIgnoreCase) { request.EntityId }, relationships, request.Depth);
        var entities = store.GetEntities();
        var affectedEntities = entities.Where(entity => graphIds.Contains(entity.Id) && entity.Id != request.EntityId).ToArray();
        var traversedRelationships = relationships
            .Where(relationship => graphIds.Contains(relationship.FromEntityId) && graphIds.Contains(relationship.ToEntityId))
            .ToArray();

        var root = store.GetEntity(request.EntityId);
        var summary = root is null
            ? "No root entity found for the supplied identifier."
            : $"{root.DisplayName} impacts {affectedEntities.Length} entities across {traversedRelationships.Length} graph edges within {request.Depth} hops.";

        return Task.FromResult(new ImpactAnalysisResult(request.EntityId, affectedEntities, traversedRelationships, summary));
    }

    public Task UpsertExtractionAsync(ExtractionResult extractionResult, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        store.Upsert(extractionResult);
        return Task.CompletedTask;
    }

    private static HashSet<string> Expand(
        HashSet<string> startingIds,
        IReadOnlyCollection<KnowledgeRelationship> relationships,
        int depth)
    {
        var visited = new HashSet<string>(startingIds, StringComparer.OrdinalIgnoreCase);
        var frontier = new HashSet<string>(startingIds, StringComparer.OrdinalIgnoreCase);

        for (var currentDepth = 0; currentDepth < Math.Max(depth, 1); currentDepth++)
        {
            var next = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            foreach (var relationship in relationships)
            {
                if (frontier.Contains(relationship.FromEntityId))
                {
                    next.Add(relationship.ToEntityId);
                }

                if (frontier.Contains(relationship.ToEntityId))
                {
                    next.Add(relationship.FromEntityId);
                }
            }

            next.ExceptWith(visited);
            if (next.Count == 0)
            {
                break;
            }

            visited.UnionWith(next);
            frontier = next;
        }

        return visited;
    }
}
