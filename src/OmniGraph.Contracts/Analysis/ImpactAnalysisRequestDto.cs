namespace OmniGraph.Contracts.Analysis;

public sealed record ImpactAnalysisRequestDto(
    string EntityId,
    int Depth = 3);
