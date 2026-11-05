using System;
using Shouldly;
using Xunit;

public class InputSafetyRegressionTests
{
    [Theory]
    [InlineData(12)]
    [InlineData(256)]
    [InlineData(4096)]
    public void Bcp47_RejectsOversizeCanonicalTagWithoutThrowing(int length)
        => Veritas.Identity.Bcp47.TryValidate(new string('a', length), out _).ShouldBeFalse();

    [Fact]
    public void HostileLengths_ReturnFailureWithoutStackGrowth()
    {
        string hostile = new string('a', 5_000_000);
        Veritas.Identity.Base58Check.TryValidate(hostile, out _).ShouldBeFalse();
        Veritas.Crypto.BitcoinAddress.TryValidate(hostile, out _).ShouldBeFalse();
        Veritas.Identity.Bcp47.TryValidate(hostile, out _).ShouldBeFalse();
        Veritas.Government.PassportNumber.TryValidate(hostile, out _).ShouldBeFalse();
        Veritas.Government.VisaNumber.TryValidate(hostile, out _).ShouldBeFalse();
        Veritas.Healthcare.Ndc.TryValidate(hostile, out _).ShouldBeFalse();
        Veritas.Healthcare.NdcPackageCode.TryValidate(hostile, out _).ShouldBeFalse();
        Veritas.Healthcare.IcdCode.TryValidate(hostile, out _).ShouldBeFalse();
        Veritas.Healthcare.RxNormIdentifier.TryValidate(hostile, out _).ShouldBeFalse();
        Veritas.Healthcare.Udi.TryValidate(hostile, out _).ShouldBeFalse();
        Veritas.Standards.IsoStandardNumber.TryValidate(hostile, out _).ShouldBeFalse();
        Veritas.Standards.IecStandardNumber.TryValidate(hostile, out _).ShouldBeFalse();
        Veritas.Standards.RfcNumber.TryValidate(hostile, out _).ShouldBeFalse();
        Veritas.Standards.UlFileNumber.TryValidate(hostile, out _).ShouldBeFalse();
        Veritas.Transportation.FlightNumber.TryValidate(hostile, out _).ShouldBeFalse();
        Veritas.Transportation.FaaNNumber.TryValidate(hostile, out _).ShouldBeFalse();
        Veritas.Transportation.ImoCallSign.TryValidate(hostile, out _).ShouldBeFalse();
        Veritas.Geospatial.DE.Flurstuecknummer.TryValidate(hostile, out _).ShouldBeFalse();
        Veritas.Geospatial.NL.KadastraleAanduiding.TryValidate(hostile, out _).ShouldBeFalse();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(255)]
    public void Base58_MaximumPayloadRoundTripsAndRejectsNextSize(byte value)
    {
        byte[] payload = new byte[1024];
        Array.Fill(payload, value);
        char[] encoded = new char[1420];
        Veritas.Algorithms.Base58Check.TryEncode(payload, encoded, out int size).ShouldBeTrue();
        byte[] decoded = new byte[1024];
        Veritas.Algorithms.Base58Check.TryDecode(encoded.AsSpan(0, size), decoded, out int count).ShouldBeTrue();
        count.ShouldBe(payload.Length);
        decoded.ShouldBe(payload);
        Veritas.Algorithms.Base58Check.TryEncode(new byte[1025], encoded, out count).ShouldBeFalse();
        count.ShouldBe(0);
    }

    [Fact]
    public void Base58Generators_RejectEveryInsufficientBuffer()
    {
        var options = new Veritas.GenerationOptions { Seed = 123 };
        Span<char> full = stackalloc char[64];
        Veritas.Identity.Base58Check.TryGenerate(options, full, out int required).ShouldBeTrue();
        for (int size = 0; size < required; size++)
        {
            Veritas.Identity.Base58Check.TryGenerate(options, new char[size], out int written).ShouldBeFalse();
            written.ShouldBe(0);
        }
        Veritas.Crypto.BitcoinAddress.TryGenerate(options, full, out required).ShouldBeTrue();
        for (int size = 0; size < required; size++)
        {
            Veritas.Crypto.BitcoinAddress.TryGenerate(options, new char[size], out int written).ShouldBeFalse();
            written.ShouldBe(0);
        }
    }

    [Fact]
    public void Base58Decoder_RejectsOversizeBeforeNumericWork()
    {
        Span<byte> destination = stackalloc byte[32];
        Veritas.Algorithms.Base58Check.TryDecode(new string('1', 4097), destination, out int written).ShouldBeFalse();
        written.ShouldBe(0);
    }

    [Fact]
    public void VariableParsers_RejectDocumentedResourceLimit()
    {
        Veritas.Healthcare.Udi.TryValidate("+" + new string('A', 4096), out _).ShouldBeFalse();
        Veritas.Standards.RfcNumber.TryValidate("RFC" + new string('1', 4094), out _).ShouldBeFalse();
        Veritas.Geospatial.NL.KadastraleAanduiding.TryValidate("ABCDE" + new string('1', 4092), out _).ShouldBeFalse();
    }

    [Fact]
    public void FixedParsers_PreserveNormalisationAndRejectExcessCharacters()
    {
        Veritas.Government.PassportNumber.TryValidate("  AB 1234567 ", out var passport).ShouldBeTrue();
        passport.Value.Value.ShouldBe("AB1234567");
        Veritas.Government.PassportNumber.TryValidate(new string('A', 100), out _).ShouldBeFalse();
        Veritas.Government.VisaNumber.TryValidate(new string('1', 100), out _).ShouldBeFalse();
        Veritas.Healthcare.Ndc.TryValidate(new string('1', 100), out _).ShouldBeFalse();
        Veritas.Healthcare.IcdCode.TryValidate(new string('A', 100), out _).ShouldBeFalse();
        Veritas.Geospatial.DE.Flurstuecknummer.TryValidate(new string('1', 100), out _).ShouldBeFalse();
    }

    [Theory]
    [InlineData("DE12\u0661")]
    [InlineData("DE12\uff11")]
    public void Iban_RejectsNonAsciiDigitsWithoutThrowing(string input)
        => Veritas.Finance.Iban.TryValidate(input, out _).ShouldBeFalse();

    [Fact]
    public void Eic_RejectsNonAsciiDigitsWithoutThrowing()
        => Veritas.Energy.Eic.TryValidate("11Y12345678901\u0661T", out _).ShouldBeFalse();

    [Fact]
    public void PlusCode_RejectsSeparatorBeyondBufferWithoutThrowing()
        => Veritas.Geospatial.PlusCode.TryValidate(new string('2', 15) + "+", out _).ShouldBeFalse();
}
