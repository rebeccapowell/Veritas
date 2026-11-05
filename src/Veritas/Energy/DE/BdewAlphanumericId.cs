using System;

namespace Veritas.Energy.DE;

/// <summary>Shared syntax and synthetic-generation helpers for BDEW's eleven-character codes.</summary>
internal static class BdewAlphanumericId
{
    internal static bool TryValidate(ReadOnlySpan<char> input, char expectedType, Span<char> buffer, out ValidationError error)
    {
        error = ValidationError.Length;
        if (input.Length > 4096 || buffer.Length < 11) return false;
        int length = 0;
        foreach (char c in input)
        {
            if (c == ' ') continue;
            if (length == 11) return false;
            if (!char.IsAsciiLetterOrDigit(c)) { error = ValidationError.Charset; return false; }
            char upper = char.ToUpperInvariant(c);
            buffer[length++] = upper;
        }
        if (length != 11) return false;
        error = ValidationError.Format;
        if (buffer[0] != expectedType || !char.IsAsciiDigit(buffer[10])) return false;
        error = ValidationError.Checksum;
        if (BdewChecksum.ComputeAlphanumeric(buffer[..10]) != buffer[10] - '0') return false;
        error = ValidationError.None;
        return true;
    }

    internal static bool TryGenerate(char type, in GenerationOptions options, Span<char> destination, out int written)
    {
        written = 0;
        if (destination.Length < 11 || type < 'A' || type > 'F') return false;
        var random = options.Seed.HasValue ? new Random(options.Seed.Value) : Random.Shared;
        const string alphabet = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        destination[0] = type;
        for (int i = 1; i < 10; i++) destination[i] = alphabet[random.Next(alphabet.Length)];
        destination[10] = (char)('0' + BdewChecksum.ComputeAlphanumeric(destination[..10]));
        written = 11;
        return true;
    }
}
