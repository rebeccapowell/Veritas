using System;
using System.Numerics;
using System.Security.Cryptography;

namespace Veritas.Algorithms;

/// <summary>Provides Base58Check encoding and decoding utilities.</summary>
internal static class Base58Check
{
    internal const int MaxPayloadLength = 1024;
    private const int MaxEncodedLength = (MaxPayloadLength + 4) * 138 / 100 + 1;
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
        if (input.IsEmpty || input.Length > MaxEncodedLength ||
            input.Length > ((long)destination.Length + 4) * 138 / 100 + 1) return false;

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

        int byteCount = num.IsZero ? 0 : num.GetByteCount(true);
        int total = zeros + byteCount;
        if (total < 4 || total > MaxPayloadLength + 4 || total - 4 > destination.Length) return false;
        Span<byte> buffer = stackalloc byte[MaxPayloadLength + 4];
        int bytesWritten = 0;
        if (!num.IsZero && !num.TryWriteBytes(buffer, out bytesWritten, isUnsigned: true, isBigEndian: true))
            return false;

        Span<byte> fullBuffer = stackalloc byte[MaxPayloadLength + 4];
        Span<byte> full = fullBuffer[..total];
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
        if (data.Length > MaxPayloadLength) return false;
        Span<byte> inputBuffer = stackalloc byte[MaxPayloadLength + 4];
        Span<byte> input = inputBuffer[..(data.Length + 4)];
        data.CopyTo(input);

        Span<byte> hash = stackalloc byte[32];
        using var sha = SHA256.Create();
        if (!sha.TryComputeHash(data, hash, out _)) return false;
        if (!sha.TryComputeHash(hash.Slice(0, 32), hash, out _)) return false;
        hash.Slice(0, 4).CopyTo(input.Slice(data.Length));

        BigInteger num = new BigInteger(input, isUnsigned: true, isBigEndian: true);
        Span<char> buffer = stackalloc char[MaxEncodedLength];
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
