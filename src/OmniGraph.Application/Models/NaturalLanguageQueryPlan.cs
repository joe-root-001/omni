namespace OmniGraph.Application.Models;

public sealed record NaturalLanguageQueryPlan(
    string InputText,
    string Intent,
    string StructuredFilter,
    string PlannedCypher,
    GraphQueryRequest Request);
