using System.Text.Json.Serialization;
using OmniGraph.Api.GraphQL;
using OmniGraph.Application.DependencyInjection;
using OmniGraph.Infrastructure.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });
builder.Services.AddOpenApi();
builder.Services.AddOmniGraphApplication();
builder.Services.AddOmniGraphInfrastructure(builder.Configuration);
builder.Services
    .AddGraphQLServer()
    .AddQueryType<GraphQuery>()
    .AddMutationType<GraphMutation>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.MapControllers();
app.MapGraphQL("/graphql");
app.MapGet("/", () => Results.Redirect("/graphql"));

app.Run();

public partial class Program;
