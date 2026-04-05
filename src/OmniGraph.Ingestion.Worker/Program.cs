using OmniGraph.Application.DependencyInjection;
using OmniGraph.Infrastructure.DependencyInjection;
using OmniGraph.Ingestion.Worker;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddOmniGraphApplication();
builder.Services.AddOmniGraphInfrastructure(builder.Configuration);
builder.Services.AddHostedService<Worker>();

var host = builder.Build();
host.Run();
