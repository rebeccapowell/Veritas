using System;
namespace Veritas.Algorithms;

/// <summary>Provides the Luhn checksum algorithm, including the standard mod 10 and generalized base-36 variants.</summary>
internal static class Luhn
{
    /// <summary>Computes the Luhn check digit for the provided numeric string (without the check digit).</summary>
    public static int ComputeCheckDigit(ReadOnlySpan<char> digits)
    {
        int sum = 0;
        bool doubleDigit = true;
        for (int i = digits.Length - 1; i >= 0; i--)
        {
            int d = digits[i] - '0';
            if ((uint)d > 9) throw new ArgumentException("Non-digit character present", nameof(digits));
            if (doubleDigit)
            {
                d *= 2;
                if (d > 9) d -= 9;
            }
            sum += d;
            doubleDigit = !doubleDigit;
        }
        return (10 - (sum % 10)) % 10;
    }

    /// <summary>Validates that the supplied numeric string satisfies the Luhn checksum.</summary>
    public static bool Validate(ReadOnlySpan<char> digits)
    {
        if (digits.IsEmpty) return false;
        int sum = 0;
        bool doubleDigit = false;
        for (int i = digits.Length - 1; i >= 0; i--)
        {
            int d = digits[i] - '0';
            if ((uint)d > 9) return false;
            if (doubleDigit)
            {
                d *= 2;
                if (d > 9) d -= 9;
            }
            sum += d;
            doubleDigit = !doubleDigit;
        }
        return sum % 10 == 0;
    }

    private static bool TryGetBase36Value(char ch, out int value)
    {
        if (ch >= '0' && ch <= '9') { value = ch - '0'; return true; }
        if (ch >= 'A' && ch <= 'Z') { value = ch - 'A' + 10; return true; }
        if (ch >= 'a' && ch <= 'z') { value = ch - 'a' + 10; return true; }
        value = 0;
        return false;
    }

    private static int CharToBase36(char ch)
    {
        if (TryGetBase36Value(ch, out int value)) return value;
        throw new ArgumentException("Non-alphanumeric character present", nameof(ch));
    }

    private static char ValueToBase36(int value)
        => value < 10 ? (char)('0' + value) : (char)('A' + value - 10);

    /// <summary>Computes the generalized Luhn (base-36) check character for the provided string.</summary>
    public static char ComputeCheckCharacterBase36(ReadOnlySpan<char> input)
    {
        int sum = 0;
        bool doubleDigit = true;
        for (int i = input.Length - 1; i >= 0; i--)
        {
            int d = CharToBase36(input[i]);
            if (doubleDigit)
            {
                d *= 2;
                if (d >= 36) d -= 35;
            }
            sum += d;
            doubleDigit = !doubleDigit;
        }
        int check = (36 - (sum % 36)) % 36;
        return ValueToBase36(check);
    }

    /// <summary>Validates that the supplied string satisfies the generalized Luhn (base-36) checksum.</summary>
    public static bool ValidateBase36(ReadOnlySpan<char> input)
    {
        if (input.IsEmpty) return false;
        int sum = 0;
        bool doubleDigit = false;
        for (int i = input.Length - 1; i >= 0; i--)
        {
            if (!TryGetBase36Value(input[i], out int d))
            {
                return false;
            }
            if (doubleDigit)
            {
                d *= 2;
                if (d >= 36) d -= 35;
            }
            sum += d;
            doubleDigit = !doubleDigit;
        }
        return sum % 36 == 0;
    }
}
