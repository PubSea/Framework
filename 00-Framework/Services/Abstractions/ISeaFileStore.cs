using PubSea.Framework.Services.Dtos;

namespace PubSea.Framework.Services.Abstractions;

public interface ISeaFileStore
{
    /// <summary>
    /// Gets a file using its relative path after root name
    /// </summary>
    /// <param name="filePath"></param>
    /// <param name="ct"></param>
    /// <returns></returns>
    Task<FileDto> GetFile(string filePath, CancellationToken ct = default);

    /// <summary>
    /// Saves file
    /// </summary>
    /// <param name="filePath"></param>
    /// <param name="contentType"></param>
    /// <param name="content"></param>
    /// <param name="ct"></param>
    /// <returns></returns>
    Task<string> SaveFile(string filePath, string contentType, byte[] content, CancellationToken ct = default);

    /// <summary>
    /// Relocates file using two relative paths
    /// </summary>
    /// <param name="oldFilePath"></param>
    /// <param name="newFilePath"></param>
    /// <param name="ct"></param>
    /// <returns></returns>
    Task RelocateFile(string oldFilePath, string newFilePath, CancellationToken ct = default);

    /// <summary>
    /// Removes file using its relative path
    /// </summary>
    /// <param name="filePath"></param>
    /// <param name="ct"></param>
    /// <returns></returns>
    Task RemoveFile(string filePath, CancellationToken ct = default);

    /// <summary>
    /// Zips a file
    /// </summary>
    /// <param name="file"></param>
    /// <param name="ct"></param>
    /// <returns></returns>
    Task<byte[]> ZipFile(FileDto file, CancellationToken ct = default);

    /// <summary>
    /// Zips multiple files
    /// </summary>
    /// <param name="files"></param>
    /// <param name="ct"></param>
    /// <returns></returns>
    Task<byte[]> ZipFiles(IEnumerable<FileDto> files, CancellationToken ct = default);

    Task<string> ConstructGetPresignedUrl(string filePath, TimeSpan? expiresIn = null, CancellationToken ct = default);

    Task<string> ConstructPutPresignedUrl(string filePath, TimeSpan? expiresIn = null, CancellationToken ct = default);
}
