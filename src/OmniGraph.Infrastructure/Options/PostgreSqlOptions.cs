namespace OmniGraph.Infrastructure.Options;

public sealed class PostgreSqlOptions
{
    public const string SectionName = "PostgreSql";

    public string ConnectionString { get; set; } =
        "Host=localhost;Port=5432;Database=omnigraph;Username=postgres;Password=postgres";
}
