using Microsoft.Extensions.Logging;
using PubSea.Framework.Services.Abstractions;
using PubSea.Framework.Services.Dtos;
using System.IO.Compression;

namespace PubSea.Framework.Services.Implementations;

internal sealed class SeaFileService : ISeaFileService
{
    private readonly ILogger<SeaFileService> _logger;

    public SeaFileService(ILogger<SeaFileService> logger)
    {
        _logger = logger;
    }

    async Task<byte[]> ISeaFileService.ZipFile(FileDto file, CancellationToken ct)
    {
        await using MemoryStream compressedFileStream = new();

        using ZipArchive zipArchive = new(compressedFileStream, ZipArchiveMode.Create, false);
        ZipArchiveEntry zipEntry = zipArchive.CreateEntry(file.FileName, CompressionLevel.Optimal);
        await using MemoryStream originalFileStream = new(file.Content);
        await using Stream zipEntryStream = zipEntry.Open();
        originalFileStream.CopyTo(zipEntryStream);

        MakeStreamAvailable(zipArchive);

        return compressedFileStream.ToArray();
    }

    async Task<byte[]> ISeaFileService.ZipFiles(IEnumerable<FileDto> files, CancellationToken ct)
    {
        await using MemoryStream compressedFileStream = new();
        using ZipArchive zipArchive = new(compressedFileStream, ZipArchiveMode.Create);

        foreach (var file in files)
        {
            ZipArchiveEntry zipEntry = zipArchive.CreateEntry(file.FileName, CompressionLevel.Optimal);
            await using MemoryStream originalFileStream = new(file.Content);
            await using Stream zipEntryStream = zipEntry.Open();
            originalFileStream.CopyTo(zipEntryStream);
        }

        MakeStreamAvailable(zipArchive);

        return compressedFileStream.ToArray();
    }

    private static void MakeStreamAvailable(ZipArchive archive)
    {
        archive.Dispose();
    }
}
