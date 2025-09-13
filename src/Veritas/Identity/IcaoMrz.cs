using System;
using Veritas.Algorithms;
using Veritas;

namespace Veritas.Identity;

/// <summary>Machine Readable Zone (MRZ) lines compliant with ICAO 9303 TD3 passports.</summary>
public readonly struct IcaoMrzValue
{
    /// <summary>The MRZ string containing two lines separated by a newline.</summary>
    public string Value { get; }
    public IcaoMrzValue(string value) => Value = value;
}

/// <summary>Provides validation and generation for ICAO 9303 MRZ (TD3) lines.</summary>
public static class IcaoMrz
{
    /// <summary>Validates TD3 (passport) MRZ consisting of two 44-character lines.</summary>
    public static bool TryValidate(ReadOnlySpan<char> input, out ValidationResult<IcaoMrzValue> result)
    {
        int nl = input.IndexOf('\n');
        if (nl <= 0 || nl != input.LastIndexOf('\n'))
        {
            result = new ValidationResult<IcaoMrzValue>(false, default, ValidationError.Length);
            return false;
        }
        var line1 = input[..nl];
        var line2 = input[(nl + 1)..];
        if (line1.Length != 44 || line2.Length != 44)
        {
            result = new ValidationResult<IcaoMrzValue>(false, default, ValidationError.Length);
            return false;
        }
        Span<char> l1 = stackalloc char[44];
        Span<char> l2 = stackalloc char[44];
        for (int i = 0; i < 44; i++)
        {
            char c1 = line1[i];
            char c2 = line2[i];
            if (c1 >= 'a' && c1 <= 'z') c1 = (char)(c1 - 32);
            if (c2 >= 'a' && c2 <= 'z') c2 = (char)(c2 - 32);
            if (!IsMrzChar(c1) || !IsMrzChar(c2))
            {
                result = new ValidationResult<IcaoMrzValue>(false, default, ValidationError.Charset);
                return false;
            }
            l1[i] = c1;
            l2[i] = c2;
        }
        if (!Mrz.Validate(l2[..9], l2[9]) ||
            !Mrz.Validate(l2.Slice(13, 6), l2[19]) ||
            !Mrz.Validate(l2.Slice(21, 6), l2[27]) ||
            !Mrz.Validate(l2.Slice(28, 14), l2[42]))
        {
            result = new ValidationResult<IcaoMrzValue>(false, default, ValidationError.Checksum);
            return false;
        }
        Span<char> comp = stackalloc char[39];
        l2[..10].CopyTo(comp); // passport number + check
        l2.Slice(13, 7).CopyTo(comp[10..]); // birth + check
        l2.Slice(21, 7).CopyTo(comp[17..]); // expiry + check
        l2.Slice(28, 15).CopyTo(comp[24..]); // personal number + check
        if (!Mrz.Validate(comp, l2[43]))
        {
            result = new ValidationResult<IcaoMrzValue>(false, default, ValidationError.Checksum);
            return false;
        }
        result = new ValidationResult<IcaoMrzValue>(true, new IcaoMrzValue(string.Concat(new string(l1), "\n", new string(l2))), ValidationError.None);
        return true;
    }

    /// <summary>Generates a sample TD3 MRZ into the provided destination span.</summary>
    public static bool TryGenerate(Span<char> destination, out int written)
        => TryGenerate(default, destination, out written);

    /// <summary>Generates a sample TD3 MRZ using the specified options.</summary>
    public static bool TryGenerate(in GenerationOptions options, Span<char> destination, out int written)
    {
        if (destination.Length < 89)
        {
            written = 0;
            return false;
        }
        var rng = options.Seed.HasValue ? new Random(options.Seed.Value) : Random.Shared;
        Span<char> line1 = destination[..44];
        Span<char> line2 = destination.Slice(45, 44);
        line1[0] = 'P';
        line1[1] = '<';
        var cc = _countryCodes[rng.Next(_countryCodes.Length)];
        line1[2] = cc[0];
        line1[3] = cc[1];
        line1[4] = cc[2];
        Fill(line1[5..], '<');
        Span<char> passport = stackalloc char[9];
        for (int i = 0; i < 9; i++) passport[i] = (char)('0' + rng.Next(10));
        passport.CopyTo(line2);
        line2[9] = (char)('0' + Mrz.Compute(passport));
        line2[10] = cc[0]; line2[11] = cc[1]; line2[12] = cc[2];
        Span<char> birth = stackalloc char[6];
        RandomDate(rng, 1950, 2005, birth);
        birth.CopyTo(line2.Slice(13, 6));
        line2[19] = (char)('0' + Mrz.Compute(birth));
        line2[20] = rng.Next(2) == 0 ? 'M' : 'F';
        Span<char> exp = stackalloc char[6];
        RandomDate(rng, 2025, 2035, exp);
        exp.CopyTo(line2.Slice(21, 6));
        line2[27] = (char)('0' + Mrz.Compute(exp));
        Fill(line2.Slice(28, 14), '<');
        line2[42] = (char)('0' + Mrz.Compute(line2.Slice(28, 14)));
        Span<char> comp = stackalloc char[39];
        line2[..10].CopyTo(comp);
        line2.Slice(13, 7).CopyTo(comp[10..]);
        line2.Slice(21, 7).CopyTo(comp[17..]);
        line2.Slice(28, 15).CopyTo(comp[24..]);
        line2[43] = (char)('0' + Mrz.Compute(comp));
        destination[44] = '\n';
        written = 89;
        return true;
    }

    private static bool IsMrzChar(char c)
        => (c >= 'A' && c <= 'Z') || (c >= '0' && c <= '9') || c == '<';

    private static void Fill(Span<char> span, char value)
    {
        for (int i = 0; i < span.Length; i++) span[i] = value;
    }

    private static void RandomDate(Random rng, int startYear, int endYear, Span<char> dest)
    {
        int year = rng.Next(startYear, endYear + 1);
        int month = rng.Next(1, 13);
        int day = rng.Next(1, 28);
        dest[0] = (char)('0' + (year / 10_00) % 10);
        dest[1] = (char)('0' + (year / 100) % 10);
        dest[2] = (char)('0' + (month / 10));
        dest[3] = (char)('0' + (month % 10));
        dest[4] = (char)('0' + (day / 10));
        dest[5] = (char)('0' + (day % 10));
    }

    private static readonly string[] _countryCodes = { "USA", "GBR", "FRA", "DEU", "CAN", "AUS" };
}

