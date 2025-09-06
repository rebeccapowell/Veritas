using System;
using System.Numerics;
using System.Security.Cryptography;

namespace Veritas.Algorithms;

/// <summary>Provides Base58Check encoding and decoding utilities.</summary>
internal static class Base58Check
{
    private const string Alphabet = "123456789ABCDEFGHJKLMNPQRSTUVWXYZabcdefghijkmnopqrstuvwxyz";

    private static readonly sbyte[] Map;

    static Base58Check()
    {
        Map = new sbyte[128];
        for (int i = 0; i < Map.Length; i++) Map[i] = -1;
        for (int i = 0; i < Alphabet.Length; i++) Map[Alphabet[i]] = (sbyte)i;
    }

    /// <summary>Decodes a Base58Check string into binary data.</summary>
    public static bool TryDecode(ReadOnlySpan<char> input, Span<byte> destination, out int written)
    {
        written = 0;
        if (input.IsEmpty) return false;

        var map = Map;
        BigInteger num = BigInteger.Zero;
        foreach (char c in input)
        {
            int v = c < map.Length ? map[c] : -1;
            if (v < 0) return false;
            num = num * 58 + v;
        }

        int zeros = 0;
        while (zeros < input.Length && input[zeros] == '1') zeros++;

        int byteCount = num.GetByteCount(true);
        Span<byte> buffer = stackalloc byte[byteCount];
        if (!num.TryWriteBytes(buffer, out int bytesWritten, isUnsigned: true, isBigEndian: true))
            return false;

        int total = zeros + bytesWritten;
        if (total < 4) return false;
        Span<byte> full = stackalloc byte[total];
        full.Slice(0, zeros).Fill(0);
        buffer.Slice(byteCount - bytesWritten, bytesWritten).CopyTo(full.Slice(zeros));

        int payloadLen = total - 4;
        Span<byte> hash = stackalloc byte[32];
        using var sha = SHA256.Create();
        if (!sha.TryComputeHash(full.Slice(0, payloadLen), hash, out _)) return false;
        if (!sha.TryComputeHash(hash.Slice(0, 32), hash, out _)) return false;
        if (!full.Slice(payloadLen, 4).SequenceEqual(hash.Slice(0, 4))) return false;

        if (destination.Length < payloadLen) return false;
        full.Slice(0, payloadLen).CopyTo(destination);
        written = payloadLen;
        return true;
    }

    /// <summary>Encodes data into a Base58Check string.</summary>
    public static bool TryEncode(ReadOnlySpan<byte> data, Span<char> destination, out int written)
    {
        written = 0;
        Span<byte> input = stackalloc byte[data.Length + 4];
        data.CopyTo(input);

        Span<byte> hash = stackalloc byte[32];
        using var sha = SHA256.Create();
        if (!sha.TryComputeHash(data, hash, out _)) return false;
        if (!sha.TryComputeHash(hash.Slice(0, 32), hash, out _)) return false;
        hash.Slice(0, 4).CopyTo(input.Slice(data.Length));

        BigInteger num = new BigInteger(input, isUnsigned: true, isBigEndian: true);
        Span<char> buffer = stackalloc char[destination.Length];
        int pos = buffer.Length;
        while (num > 0)
        {
            num = BigInteger.DivRem(num, 58, out BigInteger rem);
            buffer[--pos] = Alphabet[(int)rem];
        }

        for (int i = 0; i < input.Length && input[i] == 0; i++)
            buffer[--pos] = '1';

        int len = buffer.Length - pos;
        if (destination.Length < len) return false;
        buffer.Slice(pos, len).CopyTo(destination);
        written = len;
        return true;
    }
}
