using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Veritas.Identity;

/// <summary>A syntactically well-formed, lower-case BCP 47 language tag.</summary>
/// <example>Bcp47.TryValidate("zh-Hant-TW", out var result);</example>
public readonly struct Bcp47Value
{
    /// <summary>Gets the normalised tag.</summary>
    public string Value { get; }
    /// <summary>Creates a tag value.</summary>
    public Bcp47Value(string value) => Value = value;
    /// <summary>Returns the normalised tag.</summary>
    public override string ToString() => Value;
}

/// <summary>Checks RFC 5646 well-formed syntax, not IANA registration or subtag semantics.</summary>
/// <example>Bcp47.TryValidate("es-419", out var result);</example>
public static class Bcp47
{
    private static readonly Regex Grammar = new(
        @"\A(?:(?:[a-z]{2,3}(?:-[a-z]{3}){0,3}|[a-z]{4}|[a-z]{5,8})(?:-[a-z]{4})?(?:-(?:[a-z]{2}|[0-9]{3}))?(?:-(?:[a-z0-9]{5,8}|[0-9][a-z0-9]{3}))*(?:-[0-9a-wy-z](?:-[a-z0-9]{2,8})+)*(?:-x(?:-[a-z0-9]{1,8})+)?|x(?:-[a-z0-9]{1,8})+)\z",
        RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.NonBacktracking);

    private static readonly HashSet<string> Grandfathered = new(StringComparer.OrdinalIgnoreCase)
    {
        "en-GB-oed", "i-ami", "i-bnn", "i-default", "i-enochian", "i-hak", "i-klingon", "i-lux",
        "i-mingo", "i-navajo", "i-pwn", "i-tao", "i-tay", "i-tsu", "sgn-BE-FR", "sgn-BE-NL", "sgn-CH-DE",
        "art-lojban", "cel-gaulish", "no-bok", "no-nyn", "zh-guoyu", "zh-hakka", "zh-min", "zh-min-nan", "zh-xiang"
    };

    /// <summary>Attempts syntax validation of a tag up to 4,096 ASCII characters.</summary>
    public static bool TryValidate(ReadOnlySpan<char> input, out ValidationResult<Bcp47Value> result)
    {
        result = new ValidationResult<Bcp47Value>(false, default, ValidationError.Format);
        if (input.IsEmpty || input.Length > 4096) return false;
        for (int i = 0; i < input.Length; i++)
            if (!(char.IsAsciiLetterOrDigit(input[i]) || input[i] == '-')) return false;
        string tag = input.ToString().ToLowerInvariant();
        if (!Grandfathered.Contains(tag))
        {
            if (!Grammar.IsMatch(tag)) return false;
            string[] parts = tag.Split('-');
            ulong singletons = 0;
            var variants = new HashSet<string>(StringComparer.Ordinal);
            bool extensions = false;
            for (int i = 1; i < parts.Length; i++)
            {
                string part = parts[i];
                if (part == "x" || parts[0] == "x") break;
                if (part.Length == 1)
                {
                    int bit = part[0] <= '9' ? part[0] - '0' : part[0] - 'a' + 10;
                    ulong mask = 1UL << bit;
                    if ((singletons & mask) != 0) return false;
                    singletons |= mask;
                    extensions = true;
                }
                else if (!extensions && (part.Length >= 5 || part.Length == 4 && char.IsAsciiDigit(part[0])))
                {
                    if (!variants.Add(part)) return false;
                }
            }
        }
        result = new ValidationResult<Bcp47Value>(true, new Bcp47Value(tag), ValidationError.None);
        return true;
    }
}
