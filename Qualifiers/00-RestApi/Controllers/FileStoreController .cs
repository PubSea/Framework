using Microsoft.AspNetCore.Mvc;
using PubSea.Framework.Services.Abstractions;
using PubSea.Framework.Utility;
using System.Web;

namespace RestApiProcessor.Controllers;

[ApiController]
[Route("[controller]")]
public class FileStoreController : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Save([FromServices] ISeaFileStore fileStore,
        [FromForm] SampleFile sampleFile, CancellationToken ct)
    {
        var fileDto = await sampleFile.File.ToFileDto(ct);
        var path = $"/Dennis/Ritchie/file{fileDto.Extension}";
        await fileStore.SaveFile(path, fileDto.ContentType, fileDto.Content, ct);

        return Ok();
    }

    [HttpGet("{path}")]
    public async Task<IActionResult> ZipFile([FromServices] ISeaFileStore fileStore,
        [FromRoute] string path,
        CancellationToken ct)
    {
        var p = HttpUtility.UrlDecode(path);
        var rs = await fileStore.GetFile(p, ct);

        var zip = await fileStore.ZipFile(rs, ct);

        var fileName = $"file.zip";
        return File(zip, "application/zip", fileName);
    }

    [HttpGet("multiple")]
    public async Task<IActionResult> ZipFiles([FromServices] ISeaFileStore fileStore, CancellationToken ct)
    {
        var path1 = "/Dennis/Ritchie/file.png";
        var path2 = "/Dennis/Ritchie/file.docx";

        var rs1 = await fileStore.GetFile(path1, ct);
        var rs2 = await fileStore.GetFile(path2, ct);

        var zip = await fileStore.ZipFiles([rs1, rs2], ct);

        var fileName = $"file.zip";
        return File(zip, "application/zip", fileName);
    }
}

public sealed class SampleFile
{
    public IFormFile File { get; set; } = null!;
}