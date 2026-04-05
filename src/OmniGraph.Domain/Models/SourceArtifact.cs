using OmniGraph.Domain.Enums;

namespace OmniGraph.Domain.Models;

public sealed record SourceArtifact(
    string Id,
    ArtifactKind Kind,
    string Uri,
    string Checksum,
    DateTimeOffset CapturedAtUtc,
    Dictionary<string, string> Metadata);
