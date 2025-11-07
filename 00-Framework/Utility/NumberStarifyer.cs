using System.Text;

namespace PubSea.Framework.Utility;

public static class NumberStarifyer
{
    /// <summary>
    /// Starify a number. It is useful for hiding some part of phone number and etc.
    /// </summary>
    /// <param name="number"></param>
    /// <param name="startIndex"></param>
    /// <param name="length"></param>
    /// <returns></returns>
    public static string Starify(string number, int startIndex, int length)
    {
        var sb = new StringBuilder(number);
        sb.Remove(startIndex, length);
        sb.Insert(startIndex, string.Join("", Enumerable.Repeat("*", length)));
        return sb.ToString();
    }
}
