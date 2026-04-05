namespace OmniGraph.Domain.Models;

public sealed record SourceEvidence(
    string ArtifactId,
    string Section,
    string Snippet,
    string ExtractionRule);
