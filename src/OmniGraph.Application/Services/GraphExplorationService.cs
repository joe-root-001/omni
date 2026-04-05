using OmniGraph.Application.Abstractions;
using OmniGraph.Application.Models;
using OmniGraph.Domain.Models;

namespace OmniGraph.Application.Services;

public sealed class GraphExplorationService(
    IGraphRepository graphRepository,
    IExplainabilityService explainabilityService)
{
    public Task<KnowledgeEntity?> GetEntityAsync(string entityId, CancellationToken cancellationToken) =>
        graphRepository.GetEntityAsync(entityId, cancellationToken);

    public Task<GraphQueryResult> GetEntityNeighborhoodAsync(string entityId, int depth, CancellationToken cancellationToken) =>
        graphRepository.GetEntityNeighborhoodAsync(entityId, depth, cancellationToken);

    public Task<ImpactAnalysisResult> AnalyzeImpactAsync(ImpactAnalysisRequest request, CancellationToken cancellationToken) =>
        graphRepository.AnalyzeImpactAsync(request, cancellationToken);

    public Task<ExplainabilityResult?> ExplainRelationshipAsync(string relationshipId, CancellationToken cancellationToken) =>
        explainabilityService.ExplainRelationshipAsync(relationshipId, cancellationToken);
}
