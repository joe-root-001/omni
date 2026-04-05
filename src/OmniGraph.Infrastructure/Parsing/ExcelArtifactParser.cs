using OmniGraph.Application.Abstractions;
using OmniGraph.Domain.Enums;
using OmniGraph.Domain.Models;

namespace OmniGraph.Infrastructure.Parsing;

public sealed class ExcelArtifactParser : IArtifactParser
{
    public ArtifactKind Kind => ArtifactKind.Excel;

    public Task<ExtractionResult> ParseAsync(SourceArtifact artifact, Stream content, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var version = new VersionStamp(artifact.CapturedAtUtc, artifact.CapturedAtUtc, null, 1);
        var table = new KnowledgeEntity(
            $"table-{artifact.Id}",
            KnowledgeEntityType.Table,
            "Structured Workbook",
            artifact.Uri,
            RiskLevel.Low,
            version,
            artifact.Id,
            new Dictionary<string, string>
            {
                ["sheet"] = artifact.Metadata.GetValueOrDefault("sheet", "Sheet1"),
                ["schema"] = "transactions,user,metric"
            });

        return Task.FromResult(new ExtractionResult(artifact, new[] { table }, Array.Empty<KnowledgeRelationship>(), 0.95, Array.Empty<string>()));
    }
}
