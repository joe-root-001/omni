namespace OmniGraph.Application.Models;

public sealed record ImpactAnalysisRequest(
    string EntityId,
    int Depth);
