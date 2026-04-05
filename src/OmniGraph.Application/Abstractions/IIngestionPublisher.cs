using OmniGraph.Domain.Models;

namespace OmniGraph.Application.Abstractions;

public interface IIngestionPublisher
{
    Task PublishAsync(IngestionJob job, CancellationToken cancellationToken);
}
