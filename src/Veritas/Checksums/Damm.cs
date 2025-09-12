namespace Veritas.Checksums;

/// <summary>Provides the Damm quasigroup checksum algorithm.</summary>
public static class Damm
{
    private static readonly int[,] table = new int[,]
    {
        {0,3,1,7,5,9,8,6,4,2},
        {7,0,9,2,1,5,4,8,6,3},
        {4,2,0,6,8,7,1,3,5,9},
        {1,7,5,0,9,8,3,4,2,6},
        {6,1,2,3,0,4,5,9,7,8},
        {3,6,7,4,2,0,9,5,8,1},
        {5,8,6,9,7,2,0,1,3,4},
        {8,9,4,5,3,6,2,0,1,7},
        {9,4,3,8,6,1,7,2,0,5},
        {2,5,8,1,4,3,6,7,9,0}
    };

    /// <summary>Computes the Damm check digit for the supplied numeric string.</summary>
    public static byte Compute(ReadOnlySpan<char> digits)
    {
        int c = 0;
        foreach (var ch in digits)
        {
            int d = ch - '0';
            if ((uint)d > 9) throw new ArgumentException("Non-digit character", nameof(digits));
            c = table[c, d];
        }
        return (byte)c;
    }

    /// <summary>Validates a sequence with an appended Damm check digit.</summary>
    public static bool Validate(ReadOnlySpan<char> digitsWithCheck)
    {
        int c = 0;
        foreach (var ch in digitsWithCheck)
        {
            int d = ch - '0';
            if ((uint)d > 9) return false;
            c = table[c, d];
        }
        return c == 0;
    }

    /// <summary>Appends the Damm check digit to the destination buffer.</summary>
    public static bool Append(ReadOnlySpan<char> digits, Span<char> destination, out int written)
    {
        written = 0;
        if (destination.Length < digits.Length + 1) return false;
        digits.CopyTo(destination);
        destination[digits.Length] = (char)('0' + Compute(digits));
        written = digits.Length + 1;
        return true;
    }
}

