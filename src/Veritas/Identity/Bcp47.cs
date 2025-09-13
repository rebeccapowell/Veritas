using System;
using System.Globalization;
using Veritas;

namespace Veritas.Identity;

public readonly struct Bcp47Value
{
    public string Value { get; }
    public Bcp47Value(string value) => Value = value;
    public override string ToString() => Value;
}

/// <summary>Provides validation for BCP 47 language tags.</summary>
public static class Bcp47
{
    public static bool TryValidate(ReadOnlySpan<char> input, out ValidationResult<Bcp47Value> result)
    {
        if (input.IsEmpty)
        {
            result = new ValidationResult<Bcp47Value>(false, default, ValidationError.Length);
            return false;
        }
        Span<char> buf = stackalloc char[input.Length];
        for (int i = 0; i < input.Length; i++)
        {
            char c = input[i];
            if (c == '-') { buf[i] = '-'; continue; }
            if ((c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z'))
                buf[i] = char.ToLowerInvariant(c);
            else
            {
                result = new ValidationResult<Bcp47Value>(false, default, ValidationError.Charset);
                return false;
            }
        }
        var span = buf[..input.Length];
        int dash = span.IndexOf('-');
        if (dash == -1)
        {
            if (span.Length < 2 || span.Length > 8)
            {
                result = new ValidationResult<Bcp47Value>(false, default, ValidationError.Format);
                return false;
            }
        }
        else
        {
            var lang = span[..dash];
            var region = span[(dash + 1)..];
            if (lang.Length < 2 || lang.Length > 8 || region.Length != 2)
            {
                result = new ValidationResult<Bcp47Value>(false, default, ValidationError.Format);
                return false;
            }
            if (!(region[0] >= 'a' && region[0] <= 'z' && region[1] >= 'a' && region[1] <= 'z'))
            {
                result = new ValidationResult<Bcp47Value>(false, default, ValidationError.Charset);
                return false;
            }
        }
        var tag = new string(span);
        try
        {
            CultureInfo.GetCultureInfo(tag);
            result = new ValidationResult<Bcp47Value>(true, new Bcp47Value(tag), ValidationError.None);
            return true;
        }
        catch (CultureNotFoundException)
        {
            result = new ValidationResult<Bcp47Value>(false, default, ValidationError.Format);
            return false;
        }
    }
}
