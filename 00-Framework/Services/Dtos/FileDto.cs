namespace PubSea.Framework.Services.Dtos;

public sealed class FileDto
{
    public byte[] Content { get; init; } = [];
    public string ContentType { get; init; } = default!;
    public string FileName { get; init; } = default!;
    public string Extension { get; init; } = default!;
}
