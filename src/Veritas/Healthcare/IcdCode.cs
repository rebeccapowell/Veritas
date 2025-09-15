using System;

namespace Veritas.Healthcare;

/// <summary>Represents a validated ICD diagnostic code.</summary>
public readonly struct IcdCodeValue
{
    /// <summary>Gets the normalized ICD code.</summary>
    public string Value { get; }

    /// <summary>Initializes a new instance of the <see cref="IcdCodeValue"/> struct.</summary>
    /// <param name="value">Normalized ICD code.</param>
    public IcdCodeValue(string value) => Value = value;
}

/// <summary>Provides validation and generation for ICD-9/10/11 codes.</summary>
public static class IcdCode
{
    /// <summary>Attempts to validate the supplied ICD code.</summary>
    /// <param name="input">Candidate code.</param>
    /// <param name="result">The validation outcome including the parsed value when valid.</param>
    /// <returns><c>true</c> if validation succeeded; otherwise, <c>false</c>.</returns>
    public static bool TryValidate(ReadOnlySpan<char> input, out ValidationResult<IcdCodeValue> result)
    {
        Span<char> buf = stackalloc char[input.Length];
        int len = 0;
        foreach (var ch in input)
        {
            if (ch == ' ')
                continue;
            buf[len++] = char.ToUpperInvariant(ch);
        }
        if (len < 3)
        {
            result = new ValidationResult<IcdCodeValue>(false, default, ValidationError.Length);
            return false;
        }
        if (buf[0] >= 'A' && buf[0] <= 'Z')
        {
            if (buf[1] < '0' || buf[1] > '9' || buf[2] < '0' || buf[2] > '9')
            {
                result = new ValidationResult<IcdCodeValue>(false, default, ValidationError.Charset);
                return false;
            }
            if (len > 3)
            {
                if (buf[3] != '.')
                {
                    result = new ValidationResult<IcdCodeValue>(false, default, ValidationError.Format);
                    return false;
                }
                int rest = len - 4;
                if (rest < 1 || rest > 4)
                {
                    result = new ValidationResult<IcdCodeValue>(false, default, ValidationError.Length);
                    return false;
                }
                for (int i = 4; i < len; i++)
                {
                    var c = buf[i];
                    if (!((c >= '0' && c <= '9') || (c >= 'A' && c <= 'Z')))
                    {
                        result = new ValidationResult<IcdCodeValue>(false, default, ValidationError.Charset);
                        return false;
                    }
                }
            }
        }
        else if (buf[0] >= '0' && buf[0] <= '9')
        {
            if (buf[1] < '0' || buf[1] > '9' || buf[2] < '0' || buf[2] > '9')
            {
                result = new ValidationResult<IcdCodeValue>(false, default, ValidationError.Charset);
                return false;
            }
            if (len > 3)
            {
                if (buf[3] != '.')
                {
                    result = new ValidationResult<IcdCodeValue>(false, default, ValidationError.Format);
                    return false;
                }
                int rest = len - 4;
                if (rest < 1 || rest > 2)
                {
                    result = new ValidationResult<IcdCodeValue>(false, default, ValidationError.Length);
                    return false;
                }
                for (int i = 4; i < len; i++)
                {
                    var c = buf[i];
                    if (c < '0' || c > '9')
                    {
                        result = new ValidationResult<IcdCodeValue>(false, default, ValidationError.Charset);
                        return false;
                    }
                }
            }
        }
        else
        {
            result = new ValidationResult<IcdCodeValue>(false, default, ValidationError.Charset);
            return false;
        }
        result = new ValidationResult<IcdCodeValue>(true, new IcdCodeValue(new string(buf[..len])), ValidationError.None);
        return true;
    }

    /// <summary>Attempts to generate a random ICD code into the provided buffer.</summary>
    /// <param name="destination">Buffer receiving the generated code.</param>
    /// <param name="written">When the method returns, contains the number of characters produced.</param>
    /// <returns><c>true</c> if generation succeeded; otherwise, <c>false</c>.</returns>
    public static bool TryGenerate(Span<char> destination, out int written)
        => TryGenerate(default, destination, out written);

    /// <summary>Attempts to generate a random ICD code using the supplied options.</summary>
    /// <param name="options">Generation options.</param>
    /// <param name="destination">Buffer receiving the generated code.</param>
    /// <param name="written">When the method returns, contains the number of characters produced.</param>
    /// <returns><c>true</c> if generation succeeded; otherwise, <c>false</c>.</returns>
    public static bool TryGenerate(in GenerationOptions options, Span<char> destination, out int written)
    {
        var rng = options.Seed.HasValue ? new Random(options.Seed.Value) : Random.Shared;
        bool letter = rng.Next(2) == 0;
        if (letter)
        {
            int extra = rng.Next(0, 5);
            int total = 3 + (extra > 0 ? 1 + extra : 0);
            if (destination.Length < total)
            {
                written = 0;
                return false;
            }
            destination[0] = (char)('A' + rng.Next(26));
            destination[1] = (char)('0' + rng.Next(10));
            destination[2] = (char)('0' + rng.Next(10));
            if (extra > 0)
            {
                destination[3] = '.';
                for (int i = 0; i < extra; i++)
                {
                    int v = rng.Next(36);
                    destination[4 + i] = v < 10 ? (char)('0' + v) : (char)('A' + (v - 10));
                }
            }
            written = total;
            return true;
        }
        else
        {
            int extra = rng.Next(0, 3);
            int total = 3 + (extra > 0 ? 1 + extra : 0);
            if (destination.Length < total)
            {
                written = 0;
                return false;
            }
            for (int i = 0; i < 3; i++)
                destination[i] = (char)('0' + rng.Next(10));
            if (extra > 0)
            {
                destination[3] = '.';
                for (int i = 0; i < extra; i++)
                    destination[4 + i] = (char)('0' + rng.Next(10));
            }
            written = total;
            return true;
        }
    }
}

