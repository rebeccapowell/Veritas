using System;

namespace Veritas.Algorithms;

/// <summary>Matches the fixed-length n (digit), a (letter), c (alphanumeric) registry notation.</summary>
internal static class NationalFormat
{
    internal static bool Matches(ReadOnlySpan<char> value, ReadOnlySpan<char> pattern)
    {
        int offset = 0;
        for (int i = 0; i < pattern.Length;)
        {
            int count = 0;
            while (i < pattern.Length && pattern[i] >= '0' && pattern[i] <= '9') count = count * 10 + pattern[i++] - '0';
            if (i < pattern.Length && pattern[i] == '!') i++;
            if (count == 0 || i >= pattern.Length || count > value.Length - offset) return false;
            char kind = pattern[i++];
            for (int n = 0; n < count; n++)
            {
                char c = value[offset++];
                bool digit = c >= '0' && c <= '9';
                bool alpha = c >= 'A' && c <= 'Z';
                if (!(kind == 'n' ? digit : kind == 'a' ? alpha : kind == 'c' && (digit || alpha))) return false;
            }
        }
        return offset == value.Length;
    }
}
