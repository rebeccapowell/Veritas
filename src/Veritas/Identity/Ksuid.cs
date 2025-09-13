using System;
using System.Buffers.Binary;
using System.Numerics;
using System.Security.Cryptography;
using Veritas;

namespace Veritas.Identity;

public readonly struct KsuidValue
{
    public string Value { get; }
    public KsuidValue(string value) => Value = value;
    public override string ToString() => Value;
}

/// <summary>Provides validation and generation for KSUID identifiers.</summary>
public static class Ksuid
{
    private const string Alphabet = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";

    public static bool TryValidate(ReadOnlySpan<char> input, out ValidationResult<KsuidValue> result)
    {
        if (input.Length != 27)
        {
            result = new ValidationResult<KsuidValue>(false, default, ValidationError.Length);
            return false;
        }
        for (int i = 0; i < input.Length; i++)
        {
            if (Alphabet.IndexOf(input[i]) < 0)
            {
                result = new ValidationResult<KsuidValue>(false, default, ValidationError.Charset);
                return false;
            }
        }
        result = new ValidationResult<KsuidValue>(true, new KsuidValue(new string(input)), ValidationError.None);
        return true;
    }

    public static bool TryGenerate(in GenerationOptions options, Span<char> destination, out int written)
    {
        if (destination.Length < 27)
        {
            written = 0;
            return false;
        }
        Span<byte> bytes = stackalloc byte[20];
        uint ts = (uint)(DateTimeOffset.UtcNow.ToUnixTimeSeconds() - 1400000000); // KSUID epoch
        BinaryPrimitives.WriteUInt32BigEndian(bytes, ts);
        RandomNumberGenerator.Fill(bytes.Slice(4));
        EncodeBase62(bytes, destination);
        written = 27;
        return true;
    }

    private static void EncodeBase62(ReadOnlySpan<byte> data, Span<char> dest)
    {
        BigInteger num = new BigInteger(data, isUnsigned: true, isBigEndian: true);
        Span<char> tmp = stackalloc char[27];
        int pos = tmp.Length;
        while (num > 0)
        {
            num = BigInteger.DivRem(num, 62, out BigInteger rem);
            tmp[--pos] = Alphabet[(int)rem];
        }
        while (pos > 0) tmp[--pos] = Alphabet[0];
        tmp.CopyTo(dest);
    }
}
