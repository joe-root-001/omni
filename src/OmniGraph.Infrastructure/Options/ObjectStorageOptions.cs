namespace OmniGraph.Infrastructure.Options;

public sealed class ObjectStorageOptions
{
    public const string SectionName = "ObjectStorage";

    public string Provider { get; set; } = "LocalFileSystem";

    public string BasePath { get; set; } = "data/raw-artifacts";

    public string Bucket { get; set; } = "omnigraph-raw";
}
