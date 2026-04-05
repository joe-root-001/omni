namespace OmniGraph.Domain.Models;

public sealed record VersionStamp(
    DateTimeOffset ObservedAtUtc,
    DateTimeOffset ValidFromUtc,
    DateTimeOffset? ValidToUtc,
    long Revision);
