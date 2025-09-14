using System;

namespace Veritas.Transportation;

/// <summary>Represents a validated UIC train number.</summary>
public readonly struct TrainUicNumberValue
{
    /// <summary>Gets the normalized UIC train number.</summary>
    public string Value { get; }
    /// <summary>Initializes a new instance of the <see cref="TrainUicNumberValue"/> struct.</summary>
    /// <param name="value">Normalized UIC train number.</param>
    public TrainUicNumberValue(string value) => Value = value;
}

/// <summary>Provides validation and generation for UIC train numbers.</summary>
public static class TrainUicNumber
{
    /// <summary>Attempts to validate the supplied UIC train number.</summary>
    /// <param name="input">Candidate train number.</param>
    /// <param name="result">The validation outcome including the parsed value when valid.</param>
    /// <returns><c>true</c> if validation succeeded; otherwise, <c>false</c>.</returns>
    public static bool TryValidate(ReadOnlySpan<char> input, out ValidationResult<TrainUicNumberValue> result)
    {
        Span<char> buf = stackalloc char[12];
        int len = 0;
        foreach (var ch in input)
        {
            if (ch == ' ' || ch == '-')
                continue;
            if (ch < '0' || ch > '9')
            {
                result = new ValidationResult<TrainUicNumberValue>(false, default, ValidationError.Charset);
                return false;
            }
            if (len == 12)
            {
                result = new ValidationResult<TrainUicNumberValue>(false, default, ValidationError.Length);
                return false;
            }
            buf[len++] = ch;
        }
        if (len != 12)
        {
            result = new ValidationResult<TrainUicNumberValue>(false, default, ValidationError.Length);
            return false;
        }
        int sum = 0;
        int weight = 2;
        for (int i = 10; i >= 0; i--)
        {
            int digit = buf[i] - '0';
            sum += digit * weight;
            weight++;
            if (weight > 12)
                weight = 2;
        }
        int check = sum % 11;
        check = (11 - check) % 11;
        if (check == 10)
            check = 0;
        if (check != buf[11] - '0')
        {
            result = new ValidationResult<TrainUicNumberValue>(false, default, ValidationError.Checksum);
            return false;
        }
        result = new ValidationResult<TrainUicNumberValue>(true, new TrainUicNumberValue(new string(buf)), ValidationError.None);
        return true;
    }

    /// <summary>Attempts to generate a random UIC train number into the provided buffer.</summary>
    /// <param name="destination">Buffer receiving the generated number.</param>
    /// <param name="written">When the method returns, contains the number of characters produced.</param>
    /// <returns><c>true</c> if generation succeeded; otherwise, <c>false</c>.</returns>
    public static bool TryGenerate(Span<char> destination, out int written)
        => TryGenerate(default, destination, out written);

    /// <summary>Attempts to generate a random UIC train number using the supplied options.</summary>
    /// <param name="options">Generation options.</param>
    /// <param name="destination">Buffer receiving the generated number.</param>
    /// <param name="written">When the method returns, contains the number of characters produced.</param>
    /// <returns><c>true</c> if generation succeeded; otherwise, <c>false</c>.</returns>
    public static bool TryGenerate(in GenerationOptions options, Span<char> destination, out int written)
    {
        if (destination.Length < 12)
        {
            written = 0;
            return false;
        }
        var rng = options.Seed.HasValue ? new Random(options.Seed.Value) : Random.Shared;
        for (int i = 0; i < 11; i++)
            destination[i] = (char)('0' + rng.Next(10));
        int sum = 0;
        int weight = 2;
        for (int i = 10; i >= 0; i--)
        {
            int digit = destination[i] - '0';
            sum += digit * weight;
            weight++;
            if (weight > 12)
                weight = 2;
        }
        int check = sum % 11;
        check = (11 - check) % 11;
        if (check == 10)
            check = 0;
        destination[11] = (char)('0' + check);
        written = 12;
        return true;
    }
}

