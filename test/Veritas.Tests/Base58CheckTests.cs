using System;
using Veritas.Algorithms;
using Xunit;
using Shouldly;

public class Base58CheckTests
{
    [Fact]
    public void Decode_KnownAddress()
    {
        Span<byte> buf = stackalloc byte[32];
        Base58Check.TryDecode("1BoatSLRHtKNngkdXEeobR76b53LETtpyT", buf, out var len).ShouldBeTrue();
        len.ShouldBe(21);
        var expected = new byte[] { 0x00, 0x76, 0x80, 0xad, 0xec, 0x8e, 0xab, 0xca, 0xba, 0xc6, 0x76, 0xbe, 0x9e, 0x83, 0x85, 0x4a, 0xde, 0x0b, 0xd2, 0x2c, 0xdb };
        buf.Slice(0, len).ToArray().ShouldBe(expected);
    }

    [Fact]
    public void Encode_RoundTrip()
    {
        var payload = new byte[] { 0x00, 0x76, 0x80, 0xad, 0xec, 0x8e, 0xab, 0xca, 0xba, 0xc6, 0x76, 0xbe, 0x9e, 0x83, 0x85, 0x4a, 0xde, 0x0b, 0xd2, 0x2c, 0xdb };
        Span<char> dest = stackalloc char[50];
        Base58Check.TryEncode(payload, dest, out var w).ShouldBeTrue();
        new string(dest[..w]).ShouldBe("1BoatSLRHtKNngkdXEeobR76b53LETtpyT");
    }
}
