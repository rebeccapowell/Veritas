using Veritas.Checksums;

namespace Veritas.IP.Singapore;

/// <summary>Validation and generation for Singapore IPOS application numbers.</summary>
public static class IpApplicationNumber
{
    public static bool TryValidate(ReadOnlySpan<char> input, out ValidationResult<IpApplicationNumberValue> result)
    {
        Span<char> buf = stackalloc char[12];
        if (!Normalize(input, buf, out int len) || len != 12)
        {
            result = new ValidationResult<IpApplicationNumberValue>(false, default, ValidationError.Length);
            return false;
        }
        if (!char.IsLetter(buf[0]) || !char.IsLetter(buf[1]))
        {
            result = new ValidationResult<IpApplicationNumberValue>(false, default, ValidationError.Format);
            return false;
        }
        for (int i = 2; i < 12; i++)
        {
            if (!char.IsDigit(buf[i]))
            {
                result = new ValidationResult<IpApplicationNumberValue>(false, default, ValidationError.Format);
                return false;
            }
        }
        byte check = Damm.Compute(buf.Slice(2, 9));
        if (buf[11] != (char)('0' + check))
        {
            result = new ValidationResult<IpApplicationNumberValue>(false, default, ValidationError.Checksum);
            return false;
        }
        var value = new string(buf);
        result = new ValidationResult<IpApplicationNumberValue>(true, new IpApplicationNumberValue(value), ValidationError.None);
        return true;
    }

    public static bool TryGenerate(in GenerationOptions options, Span<char> destination, out int written)
    {
        written = 0;
        if (destination.Length < 12) return false;
        var rng = options.Seed.HasValue ? new Random(options.Seed.Value) : Random.Shared;
        destination[0] = (char)('A' + rng.Next(26));
        destination[1] = (char)('A' + rng.Next(26));
        for (int i = 2; i < 11; i++)
            destination[i] = (char)('0' + rng.Next(10));
        destination[11] = (char)('0' + Damm.Compute(destination.Slice(2, 9)));
        written = 12;
        return true;
    }

    private static bool Normalize(ReadOnlySpan<char> input, Span<char> dest, out int len)
    {
        len = 0;
        foreach (var ch in input)
        {
            if (ch == ' ') continue;
            if (ch == '-') break; // stop at sub-designation
            if (char.IsLetter(ch))
            {
                if (len >= dest.Length) { len = 0; return false; }
                dest[len++] = char.ToUpperInvariant(ch);
            }
            else if (char.IsDigit(ch))
            {
                if (len >= dest.Length) { len = 0; return false; }
                dest[len++] = ch;
            }
            else
            {
                len = 0;
                return false;
            }
        }
        return true;
    }
}
