using Microsoft.Extensions.Logging;
using Minio;
using Minio.DataModel.Args;
using PubSea.Framework.Services.Abstractions;
using PubSea.Framework.Services.Dtos;
using System.IO.Compression;

namespace PubSea.Framework.Services.Implementations;

internal sealed class SeaFileStore : ISeaFileStore
{
    private readonly string _bucketName;
    private readonly IMinioClient _client;
    private readonly ILogger<SeaFileStore> _logger;

    public SeaFileStore(ILogger<SeaFileStore> logger,
        Minio.AspNetCore.IMinioClientFactory minioClientFactory, SeaFileStoreConfig minioConfig)
    {
        _logger = logger;
        _client = minioClientFactory.CreateClient();
        _bucketName = minioConfig.RootName;
    }

    async Task<FileDto> ISeaFileStore.GetFile(string filePath, CancellationToken ct)
    {
        using var filestream = new MemoryStream();

        var rq = new GetObjectArgs()
            .WithBucket(_bucketName)
            .WithObject(filePath)
            .WithCallbackStream(async (stream, token) => await stream.CopyToAsync(filestream, token));

        var rs = await _client.GetObjectAsync(rq, ct);

        return new FileDto
        {
            Content = filestream.ToArray(),
            ContentType = rs.ContentType,
            FileName = rs.ObjectName,
        };
    }

    async Task<string> ISeaFileStore.SaveFile(string filePath, string contentType, byte[] content, CancellationToken ct)
    {
        using var filestream = new MemoryStream();
        filestream.Write(content, 0, content.Length);

        await SaveFileInObjectStore(filePath, contentType, filestream, ct);

        return $"{_bucketName}/{filePath}";
    }

    async Task ISeaFileStore.RelocateFile(string oldFilePath, string newFilePath, CancellationToken ct)
    {
        var oldObjectName = oldFilePath[(oldFilePath.IndexOf('/') + 1)..];
        var newObjectName = newFilePath[(newFilePath.IndexOf('/') + 1)..];

        var args = new CopyObjectArgs()
            .WithBucket(_bucketName)
            .WithCopyObjectSource(new CopySourceObjectArgs().WithBucket(_bucketName).WithObject(oldObjectName))
            .WithObject(newObjectName);

        await _client.CopyObjectAsync(args, ct);
    }

    async Task ISeaFileStore.RemoveFile(string filePath, CancellationToken ct)
    {
        var oldObjectName = filePath[(filePath.IndexOf('/') + 1)..];

        var args = new RemoveObjectArgs()
            .WithBucket(_bucketName)
            .WithObject(oldObjectName);

        await _client.RemoveObjectAsync(args, ct);
    }

    async Task<byte[]> ISeaFileStore.ZipFile(FileDto file, CancellationToken ct)
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

    async Task<byte[]> ISeaFileStore.ZipFiles(IEnumerable<FileDto> files, CancellationToken ct)
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

    async Task<string> ISeaFileStore.ConstructGetPresignedUrl(string filePath, TimeSpan? expiresIn, CancellationToken ct)
    {
        var presignArgs = new PresignedGetObjectArgs()
            .WithBucket(_bucketName)
            .WithObject(filePath)
            .WithExpiry(expiresIn?.Seconds ?? 5 * 60);

        return await _client.PresignedGetObjectAsync(presignArgs);
    }

    async Task<string> ISeaFileStore.ConstructPutPresignedUrl(string filePath, TimeSpan? expiresIn, CancellationToken ct)
    {
        var presignArgs = new PresignedPutObjectArgs()
            .WithBucket(_bucketName)
            .WithObject(filePath)
            .WithExpiry(expiresIn?.Seconds ?? 5 * 60);

        return await _client.PresignedPutObjectAsync(presignArgs);
    }

    private static void MakeStreamAvailable(ZipArchive archive)
    {
        archive.Dispose();
    }

    private async Task SaveFileInObjectStore(string filePath, string contentType, Stream content, CancellationToken ct)
    {
        content.Seek(0, SeekOrigin.Begin);

        _logger.LogFileSavingStarted(filePath, contentType);

        var args = new PutObjectArgs()
            .WithBucket(_bucketName)
            .WithObject(filePath)
            .WithStreamData(content)
            .WithObjectSize(content.Length)
            .WithContentType(contentType);

        await _client.PutObjectAsync(args, ct);

        _logger.LogFileSavingFinished(filePath, contentType);
    }
}
