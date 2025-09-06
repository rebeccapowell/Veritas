using System;

namespace Veritas.Algorithms;

/// <summary>VIN transliteration and checksum helpers.</summary>
internal static class VinMap
{
    private static readonly int[] Weights = {8,7,6,5,4,3,2,10,0,9,8,7,6,5,4,3,2};

    /// <summary>Validates a VIN string including its check digit.</summary>
    public static bool Validate(ReadOnlySpan<char> vin)
    {
        if (vin.Length != 17) return false;
        int sum = 0;
        for (int i = 0; i < 17; i++)
        {
            int v = Value(vin[i]);
            if (v < 0) return false;
            sum += v * Weights[i];
        }
        int rem = sum % 11;
        char check = rem == 10 ? 'X' : (char)('0' + rem);
        return check == char.ToUpperInvariant(vin[8]);
    }

    /// <summary>Computes the VIN check digit for the 17-character VIN.</summary>
    public static char ComputeCheckDigit(ReadOnlySpan<char> vin)
    {
        if (vin.Length != 17) throw new ArgumentException("VIN must be 17 chars", nameof(vin));
        int sum = 0;
        for (int i = 0; i < 17; i++)
        {
            int v = Value(vin[i]);
            if (v < 0) throw new ArgumentException("Invalid VIN character", nameof(vin));
            sum += v * Weights[i];
        }
        int rem = sum % 11;
        return rem == 10 ? 'X' : (char)('0' + rem);
    }

    private static int Value(char c)
    {
        if (c >= '0' && c <= '9') return c - '0';
        c = char.ToUpperInvariant(c);
        return c switch
        {
            'A' => 1,
            'B' => 2,
            'C' => 3,
            'D' => 4,
            'E' => 5,
            'F' => 6,
            'G' => 7,
            'H' => 8,
            'J' => 1,
            'K' => 2,
            'L' => 3,
            'M' => 4,
            'N' => 5,
            'P' => 7,
            'R' => 9,
            'S' => 2,
            'T' => 3,
            'U' => 4,
            'V' => 5,
            'W' => 6,
            'X' => 7,
            'Y' => 8,
            'Z' => 9,
            _ => -1
        };
    }
}
