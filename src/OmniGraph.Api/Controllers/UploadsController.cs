using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using OmniGraph.Api.Mappings;
using OmniGraph.Application.Commands;
using OmniGraph.Application.Services;
using OmniGraph.Domain.Enums;
using OmniGraph.Infrastructure.Options;
using IOPath = System.IO.Path;

namespace OmniGraph.Api.Controllers;

[ApiController]
[Route("api/uploads")]
public sealed class UploadsController(
    IWebHostEnvironment environment,
    IOptions<ObjectStorageOptions> objectStorageOptions,
    IngestionOrchestrator orchestrator) : ControllerBase
{
    [HttpPost]
    [Consumes("multipart/form-data")]
    [ProducesResponseType(typeof(UploadArtifactResponse), StatusCodes.Status202Accepted)]
    [RequestSizeLimit(104_857_600)]
    public async Task<ActionResult<UploadArtifactResponse>> UploadAsync(
        [FromForm] UploadArtifactRequest request,
        CancellationToken cancellationToken)
    {
        if (request.File is null || request.File.Length == 0)
        {
            return BadRequest("A file is required.");
        }

        var artifactKind = TryInferArtifactKind(request.File.FileName);
        if (artifactKind is null)
        {
            return BadRequest("Unsupported file type. Supported: Excel, PDF, image, log, and code files.");
        }

        var basePath = ResolveBasePath(objectStorageOptions.Value.BasePath, environment.ContentRootPath);
        Directory.CreateDirectory(basePath);

        var storedFileName = $"{Guid.NewGuid():N}_{IOPath.GetFileName(request.File.FileName)}";
        var storedPath = IOPath.Combine(basePath, storedFileName);

        await using (var stream = System.IO.File.Create(storedPath))
        {
            await request.File.CopyToAsync(stream, cancellationToken);
        }

        var job = await orchestrator.SubmitAsync(
            new SubmitIngestionCommand(
                ArtifactUri: storedPath,
                ArtifactKind: artifactKind.Value,
                RequestedBy: request.RequestedBy,
                CorrelationId: request.CorrelationId,
                Metadata: new Dictionary<string, string>
                {
                    ["provider"] = objectStorageOptions.Value.Provider,
                    ["originalFileName"] = request.File.FileName,
                    ["contentType"] = request.File.ContentType ?? "application/octet-stream"
                }),
            cancellationToken);

        return Accepted(
            $"/api/ingestion/jobs/{job.Id}",
            new UploadArtifactResponse(
                job.ToResponse(),
                storedPath,
                artifactKind.Value.ToString()));
    }

    private static string ResolveBasePath(string configuredPath, string contentRootPath) =>
        IOPath.IsPathRooted(configuredPath)
            ? configuredPath
            : IOPath.Combine(contentRootPath, configuredPath);

    private static ArtifactKind? TryInferArtifactKind(string fileName)
    {
        var extension = IOPath.GetExtension(fileName).ToLowerInvariant();

        if (extension is ".xlsx" or ".xls")
        {
            return ArtifactKind.Excel;
        }

        if (extension == ".pdf")
        {
            return ArtifactKind.Pdf;
        }

        if (extension is ".png" or ".jpg" or ".jpeg" or ".tiff" or ".bmp")
        {
            return ArtifactKind.Image;
        }

        if (extension is ".log" or ".txt")
        {
            return ArtifactKind.Log;
        }

        if (extension is ".cs" or ".js" or ".ts" or ".py" or ".java" or ".go")
        {
            return ArtifactKind.Code;
        }

        return null;
    }
}

public sealed record UploadArtifactRequest(
    IFormFile File,
    string RequestedBy,
    string? CorrelationId);

public sealed record UploadArtifactResponse(
    OmniGraph.Contracts.Ingestion.IngestionJobResponse Job,
    string StoredPath,
    string ArtifactKind);
