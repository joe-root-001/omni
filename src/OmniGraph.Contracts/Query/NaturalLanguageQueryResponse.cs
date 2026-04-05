using OmniGraph.Contracts.Graph;

namespace OmniGraph.Contracts.Query;

public sealed record NaturalLanguageQueryResponse(
    string Intent,
    string StructuredFilter,
    GraphQueryResponse Result);
