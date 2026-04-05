using OmniGraph.Application.Models;

namespace OmniGraph.Application.Abstractions;

public interface INaturalLanguageQueryTranslator
{
    Task<NaturalLanguageQueryPlan> TranslateAsync(string input, CancellationToken cancellationToken);
}
