namespace Veritas.Checksums;

/// <summary>Provides the Verhoeff check digit algorithm.</summary>
public static class Verhoeff
{
    private static readonly int[,] d = new int[,]
    {
        {0,1,2,3,4,5,6,7,8,9},
        {1,2,3,4,0,6,7,8,9,5},
        {2,3,4,0,1,7,8,9,5,6},
        {3,4,0,1,2,8,9,5,6,7},
        {4,0,1,2,3,9,5,6,7,8},
        {5,9,8,7,6,0,4,3,2,1},
        {6,5,9,8,7,1,0,4,3,2},
        {7,6,5,9,8,2,1,0,4,3},
        {8,7,6,5,9,3,2,1,0,4},
        {9,8,7,6,5,4,3,2,1,0}
    };

    private static readonly int[,] p = new int[,]
    {
        {0,1,2,3,4,5,6,7,8,9},
        {1,5,7,6,2,8,3,0,9,4},
        {5,8,0,3,7,9,6,1,4,2},
        {8,9,1,6,0,4,3,5,2,7},
        {9,4,5,3,1,2,6,8,7,0},
        {4,2,8,6,5,7,3,9,0,1},
        {2,7,9,3,8,0,6,4,1,5},
        {7,0,4,6,9,1,3,2,5,8}
    };

    private static readonly int[] inv = {0,4,3,2,1,5,6,7,8,9};

    /// <summary>Computes the Verhoeff check digit for the supplied numeric string.</summary>
    /// <param name="digits">Digits without the check digit.</param>
    /// <returns>The computed check digit 0-9.</returns>
    public static byte Compute(ReadOnlySpan<char> digits)
    {
        int c = 0;
        for (int i = 0; i < digits.Length; i++)
        {
            int dVal = digits[digits.Length - 1 - i] - '0';
            if ((uint)dVal > 9) throw new ArgumentException("Non-digit character", nameof(digits));
            c = d[c, p[(i + 1) % 8, dVal]];
        }
        return (byte)inv[c];
    }

    /// <summary>Validates a sequence with an appended Verhoeff check digit.</summary>
    /// <param name="digitsWithCheck">Digits including the check digit as the final character.</param>
    /// <returns><c>true</c> if the sequence is valid; otherwise <c>false</c>.</returns>
    public static bool Validate(ReadOnlySpan<char> digitsWithCheck)
    {
        int c = 0;
        for (int i = 0; i < digitsWithCheck.Length; i++)
        {
            int dVal = digitsWithCheck[digitsWithCheck.Length - 1 - i] - '0';
            if ((uint)dVal > 9) return false;
            c = d[c, p[i % 8, dVal]];
        }
        return c == 0;
    }

    /// <summary>Appends the Verhoeff check digit to the specified destination buffer.</summary>
    /// <param name="digits">Digits without the check digit.</param>
    /// <param name="destination">Buffer that receives digits plus check digit.</param>
    /// <param name="written">Number of characters written.</param>
    /// <returns><c>true</c> if the destination had sufficient space.</returns>
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

