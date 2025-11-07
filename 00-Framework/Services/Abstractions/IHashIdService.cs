namespace PubSea.Framework.Services.Abstractions;

/// <summary>
/// This is a hashid service which can be used in order to encode and decode ids provided in rest urls so that 
/// the end user can NOT find the next url providing a specific resource
/// </summary>
public interface IHashIdService
{
    /// <summary>
    /// Encodes an integer and returns a code
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    string Encode(int value);

    /// <summary>
    /// Encodes a long number and returns a code
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    string EncodeLong(long value);

    /// <summary>
    /// Decodes a code to an integer
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    int Decode(string value);

    /// <summary>
    /// Decodes a code to a long number
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    long DecodeLong(string value);

    /// <summary>
    /// Generates a fake hash id
    /// </summary>
    /// <returns></returns>
    string GenerateFakeHashId();

    /// <summary>
    /// Checks if given id is fake or not
    /// </summary>
    /// <param name="hashId"></param>
    /// <returns></returns>
    bool IsFakeHashId(string? hashId);
}
