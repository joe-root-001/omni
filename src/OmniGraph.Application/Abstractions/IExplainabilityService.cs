using OmniGraph.Application.Models;

namespace OmniGraph.Application.Abstractions;

public interface IExplainabilityService
{
    Task<ExplainabilityResult?> ExplainRelationshipAsync(string relationshipId, CancellationToken cancellationToken);
}
