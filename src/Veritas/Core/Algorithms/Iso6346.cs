using System;

namespace Veritas.Algorithms;

/// <summary>ISO 6346 container code check digit calculator.</summary>
internal static class Iso6346Algorithm
{
    private static readonly int[] CharValues = new int[26]
    {
        10,12,13,14,15,16,17,18,19,20,21,23,24,25,26,27,28,29,30,31,32,34,35,36,37,38
    };

    public static int Compute(ReadOnlySpan<char> ownerAndSerial)
    {
        int sum = 0;
        int weight = 1;
        for (int i = 0; i < ownerAndSerial.Length; i++)
        {
            char c = ownerAndSerial[i];
            int v;
            if (c >= '0' && c <= '9') v = c - '0';
            else
            {
                c = char.ToUpperInvariant(c);
                v = CharValues[c - 'A'];
            }
            sum += v * weight;
            weight <<= 1;
        }
        int rem = sum % 11;
        rem %= 10;
        return rem;
    }

    public static bool Validate(ReadOnlySpan<char> code)
    {
        if (code.Length != 11) return false;
        int check = code[10] - '0';
        if ((uint)check > 9) return false;
        return Compute(code[..10]) == check;
    }
}
