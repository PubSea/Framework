using PubSea.Framework.Services.Dtos;

namespace PubSea.Framework.Services.Abstractions;

public interface ISeaFileService
{
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
}
