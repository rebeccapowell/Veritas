using System;

namespace Veritas.Legal.US;

/// <summary>Validation and generation for baseline US court case numbers.</summary>
/// <example>
/// <code language="csharp">
/// Span&lt;char&gt; dst = stackalloc char[11];
/// CourtCaseNumber.TryGenerate(default, dst, out _);
/// </code>
/// </example>
public static class CourtCaseNumber
{
    private static readonly string[] _types = { "CV", "CR", "MC" };

    /// <summary>Validates the supplied court case number.</summary>
    public static bool TryValidate(ReadOnlySpan<char> input, out ValidationResult<CourtCaseNumberValue> result)
    {
        Span<char> buf = stackalloc char[11];
        int len = 0;
        foreach (var ch in input)
        {
            if (len >= 11) { result = new ValidationResult<CourtCaseNumberValue>(false, default, ValidationError.Length); return false; }
            buf[len++] = ch;
        }
        if (len != 11 || buf[2] != '-' || buf[8] != ' ')
        {
            result = new ValidationResult<CourtCaseNumberValue>(false, default, ValidationError.Format);
            return false;
        }
        for (int i = 0; i < 2; i++) if (buf[i] < '0' || buf[i] > '9') { result = new ValidationResult<CourtCaseNumberValue>(false, default, ValidationError.Charset); return false; }
        for (int i = 3; i < 8; i++) if (buf[i] < '0' || buf[i] > '9') { result = new ValidationResult<CourtCaseNumberValue>(false, default, ValidationError.Charset); return false; }
        if (!char.IsUpper(buf[9]) || !char.IsUpper(buf[10])) { result = new ValidationResult<CourtCaseNumberValue>(false, default, ValidationError.Charset); return false; }
        result = new ValidationResult<CourtCaseNumberValue>(true, new CourtCaseNumberValue(new string(buf)), ValidationError.None);
        return true;
    }

    /// <summary>Generates a court case number into the destination span.</summary>
    public static bool TryGenerate(Span<char> destination, out int written)
        => TryGenerate(default, destination, out written);

    /// <summary>Generates a court case number using the specified options.</summary>
    public static bool TryGenerate(in GenerationOptions options, Span<char> destination, out int written)
    {
        if (destination.Length < 11) { written = 0; return false; }
        var rng = options.Seed.HasValue ? new Random(options.Seed.Value) : Random.Shared;
        int year = rng.Next(0, 100);
        destination[0] = (char)('0' + year / 10);
        destination[1] = (char)('0' + year % 10);
        destination[2] = '-';
        int seq = rng.Next(0, 100000);
        for (int i = 0; i < 5; i++) { destination[7 - i] = (char)('0' + seq % 10); seq /= 10; }
        destination[8] = ' ';
        var type = _types[rng.Next(_types.Length)];
        destination[9] = type[0];
        destination[10] = type[1];
        written = 11;
        return true;
    }
}
