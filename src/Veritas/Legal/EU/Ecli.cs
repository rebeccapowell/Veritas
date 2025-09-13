using System;

namespace Veritas.Legal.EU;

/// <summary>Validation and generation for European Case Law Identifiers (ECLI).</summary>
/// <example>
/// <code language="csharp">
/// Span&lt;char&gt; dst = stackalloc char[30];
/// Ecli.TryGenerate(default, dst, out _);
/// </code>
/// </example>
public static class Ecli
{
    private static readonly string[] _countries = { "NL", "DE", "BE" };
    private static readonly string[] _courts = { "HR", "GH", "PHR" };

    /// <summary>Validates the supplied ECLI.</summary>
    public static bool TryValidate(ReadOnlySpan<char> input, out ValidationResult<EcliValue> result)
    {
        Span<char> buf = stackalloc char[32];
        int len = 0;
        foreach (var ch in input)
        {
            if (ch == ' ') continue;
            if (len >= 32) { result = new ValidationResult<EcliValue>(false, default, ValidationError.Length); return false; }
            char c = ch;
            if (c >= 'a' && c <= 'z') c = (char)(c - 32);
            buf[len++] = c;
        }
        int p1 = IndexOf(buf, len, 0);
        int p2 = IndexOf(buf, len, p1 + 1);
        int p3 = IndexOf(buf, len, p2 + 1);
        int p4 = IndexOf(buf, len, p3 + 1);
        if (p1 <= 0 || p2 <= p1 + 1 || p3 <= p2 + 1 || p4 <= p3 + 1 || p4 >= len)
        {
            result = new ValidationResult<EcliValue>(false, default, ValidationError.Format);
            return false;
        }
        if (!buf[..p1].SequenceEqual("ECLI")) { result = new ValidationResult<EcliValue>(false, default, ValidationError.Format); return false; }
        var cc = buf.Slice(p1 + 1, p2 - p1 - 1);
        if (cc.Length != 2 || !IsLetters(cc)) { result = new ValidationResult<EcliValue>(false, default, ValidationError.Format); return false; }
        var court = buf.Slice(p2 + 1, p3 - p2 - 1);
        if (court.Length == 0 || !IsAlphaNum(court)) { result = new ValidationResult<EcliValue>(false, default, ValidationError.Format); return false; }
        var year = buf.Slice(p3 + 1, p4 - p3 - 1);
        if (year.Length != 4 || !IsDigits(year)) { result = new ValidationResult<EcliValue>(false, default, ValidationError.Format); return false; }
        var id = buf.Slice(p4 + 1, len - p4 - 1);
        if (id.Length == 0 || !IsAlphaNum(id)) { result = new ValidationResult<EcliValue>(false, default, ValidationError.Format); return false; }
        result = new ValidationResult<EcliValue>(true, new EcliValue(new string(buf[..len])), ValidationError.None);
        return true;
    }

    /// <summary>Generates an ECLI into the destination span.</summary>
    public static bool TryGenerate(Span<char> destination, out int written)
        => TryGenerate(default, destination, out written);

    /// <summary>Generates an ECLI using the specified options.</summary>
    public static bool TryGenerate(in GenerationOptions options, Span<char> destination, out int written)
    {
        var rng = options.Seed.HasValue ? new Random(options.Seed.Value) : Random.Shared;
        var country = _countries[rng.Next(_countries.Length)];
        var court = _courts[rng.Next(_courts.Length)];
        int year = rng.Next(1990, DateTime.UtcNow.Year + 1);
        int id = rng.Next(0, 100000);
        var s = $"ECLI:{country}:{court}:{year}:{id:D4}";
        if (destination.Length < s.Length) { written = 0; return false; }
        s.AsSpan().CopyTo(destination);
        written = s.Length;
        return true;
    }

    private static bool IsLetters(ReadOnlySpan<char> s)
    {
        foreach (var c in s) if (c < 'A' || c > 'Z') return false; return true;
    }
    private static bool IsDigits(ReadOnlySpan<char> s)
    {
        foreach (var c in s) if (c < '0' || c > '9') return false; return true;
    }
    private static bool IsAlphaNum(ReadOnlySpan<char> s)
    {
        foreach (var c in s)
        {
            if (c >= 'A' && c <= 'Z') continue;
            if (c >= '0' && c <= '9') continue;
            return false;
        }
        return true;
    }

    private static int IndexOf(Span<char> s, int len, int start)
    {
        for (int i = start; i < len; i++) if (s[i] == ':') return i;
        return -1;
    }
}
