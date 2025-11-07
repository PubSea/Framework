using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.StaticFiles;
using PubSea.Framework.Services.Dtos;

namespace PubSea.Framework.Utility;

public static class FileHelper
{
    public static async Task<FileDto> ToFileDto(this IFormFile file, CancellationToken ct = default)
    {
        using var stream = new MemoryStream();
        await file.CopyToAsync(stream, ct);
        var content = stream.ToArray();

        return new FileDto
        {
            ContentType = file.ContentType,
            FileName = file.FileName,
            Content = content,
            Extension = Path.GetExtension(file.FileName)
        };
    }

    public static string GetContentType(string fileName)
    {
        var isValidContentType = new FileExtensionContentTypeProvider()
            .TryGetContentType(fileName, out var contentType);

        if (!isValidContentType || string.IsNullOrWhiteSpace(contentType))
        {
            contentType = "application/octet-stream";
        }

        return contentType;
    }

    public static string GetExtension(string fileName)
    {
        return Path.GetExtension(fileName);
    }
}
