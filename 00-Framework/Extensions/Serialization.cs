using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;

namespace PubSea.Framework.Extensions;

public static class Serialization
{
    /// <summary>
    /// Serializes object to byte array
    /// </summary>
    /// <param name="objectToSerialize"></param>
    /// <returns></returns>
    public static byte[]? ToByteArray(this object objectToSerialize)
    {
        if (objectToSerialize is null)
        {
            return null;
        }

        return Encoding.Default.GetBytes(JsonSerializer.Serialize(objectToSerialize, new JsonSerializerOptions
        {
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            DictionaryKeyPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = false,
        }));
    }


    /// <summary>
    /// Deserializes byte array to object
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="arrayToDeserialize"></param>
    /// <returns></returns>
    public static T? FromByteArray<T>(this byte[] arrayToDeserialize) where T : class
    {
        if (arrayToDeserialize is null)
        {
            return default;
        }

        return JsonSerializer.Deserialize<T>(Encoding.Default.GetString(arrayToDeserialize));
    }
}

