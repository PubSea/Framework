using System.Text;

namespace PubSea.Framework.Utility;

public static class StringHelper
{
    private static readonly Dictionary<char, char> _characterMaps = new() {
        { 'ي', 'ی' }, { 'ك', 'ک' }, { '?', '؟' }, 
        { '\u06F0', '0' }, { '\u06F1', '1' }, { '\u06F2', '2' }, { '\u06F3', '3' }, { '\u06F4', '4' }, { '\u06F5', '5' }, { '\u06F6', '6' }, { '\u06F7', '7' }, { '\u06F8', '8' }, { '\u06F9', '9' },
        { '\u0660', '0' }, { '\u0661', '1' }, { '\u0662', '2' }, { '\u0663', '3' }, { '\u0664', '4' }, { '\u0665', '5' }, { '\u0666', '6' }, { '\u0667', '7' }, { '\u0668', '8' }, { '\u0669', '9' },
    };

    /// <summary>
    /// Correct spelling for a string
    /// </summary>
    /// <param name="text"></param>
    /// <returns></returns>
    public static string CorrectSpelling(this string text)
    {
        return DoCorrecting(text);
    }

    /// <summary>
    /// Correct spelling for a string
    /// </summary>
    /// <param name="text"></param>
    /// <returns></returns>
    public static string CorrectSpelling(this char[] text)
    {
        return DoCorrecting(text);
    }

    /// <summary>
    /// Correct spelling for a string
    /// </summary>
    /// <param name="text"></param>
    /// <returns></returns>
    public static string CorrectSpelling(this ReadOnlySpan<char> text)
    {
        return DoCorrecting(text);
    }

    private static string DoCorrecting(ReadOnlySpan<char> text)
    {
        StringBuilder sb = new(text.Length);

        foreach (var @char in text)
        {
            if (Equals(@char, 'ـ'))
            {
                continue;
            }

            if (_characterMaps.TryGetValue(@char, out var ch))
            {
                sb.Append(ch);
            }
            else
            {
                sb.Append(@char);
            }
        }

        return sb.ToString();
    }
}
