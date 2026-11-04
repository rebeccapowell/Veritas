using System;
using Shouldly;
using Veritas;
using Veritas.Identity;
using Xunit;

public class ModernFormatsRegressionTests
{
    [Fact]
    public void TimestampBoundariesAndSeededUlidsAreExplicit()
    {
        Span<char> buffer = stackalloc char[27];
        Ulid.TryGenerate(new GenerationOptions { Seed = 17, Timestamp = DateTimeOffset.UnixEpoch.AddMilliseconds(-1) }, buffer, out int written).ShouldBeFalse();
        written.ShouldBe(0);
        Ulid.TryGenerate(new GenerationOptions { Seed = 17 }, buffer, out written).ShouldBeTrue();
        string first = new string(buffer[..written]);
        first[..10].ShouldBe("0000000000");
        Ulid.TryGenerate(new GenerationOptions { Seed = 17 }, buffer, out written).ShouldBeTrue();
        new string(buffer[..written]).ShouldBe(first);
        Ulid.TryGenerate(new GenerationOptions { Timestamp = DateTimeOffset.MaxValue }, buffer, out _).ShouldBeTrue();
        DateTimeOffset epoch = DateTimeOffset.FromUnixTimeSeconds(1400000000);
        Ksuid.TryGenerate(new GenerationOptions { Timestamp = epoch.AddSeconds(-1) }, buffer, out written).ShouldBeFalse();
        written.ShouldBe(0);
        Ksuid.TryGenerate(new GenerationOptions { Timestamp = epoch }, buffer, out _).ShouldBeTrue();
        Ksuid.TryGenerate(new GenerationOptions { Timestamp = epoch.AddSeconds(uint.MaxValue) }, buffer, out _).ShouldBeTrue();
        Ksuid.TryGenerate(new GenerationOptions { Timestamp = epoch.AddSeconds((long)uint.MaxValue + 1) }, buffer, out written).ShouldBeFalse();
        written.ShouldBe(0);
    }

    [Theory]
    [InlineData("849VCWC8+R9", true)]
    [InlineData("849V0000+", true)]
    [InlineData("CWC8+R9", true)]
    [InlineData("+R9", true)]
    [InlineData("849VCWC8+R", false)]
    [InlineData("XX9VCWC8+R9", false)]
    [InlineData("00000000+", false)]
    public void PlusCode_HandlesFullShortAndPaddedCodes(string input, bool expected)
        => Veritas.Geospatial.PlusCode.TryValidate(input, out _).ShouldBe(expected);

    [Fact]
    public void PlusCode_GeneratesTenSignificantCharacters()
    {
        Span<char> buffer = stackalloc char[16];
        for (int seed = 0; seed < 100; seed++)
        {
            Veritas.Geospatial.PlusCode.TryGenerate(new GenerationOptions { Seed = seed }, buffer, out int written).ShouldBeTrue();
            written.ShouldBe(11);
            const string alphabet = "23456789CFGHJMPQRVWX";
            alphabet.IndexOf(buffer[0]).ShouldBeLessThan(9);
            alphabet.IndexOf(buffer[1]).ShouldBeLessThan(18);
        }
    }

    [Fact]
    public void Kid_Mod11SeedSweepNeverReturnsAnInvalidReference()
    {
        Span<char> buffer = stackalloc char[25];
        for (int seed = 0; seed < 1000; seed++)
        {
            bool generated = Veritas.Tax.NO.Kid.TryGenerate(Veritas.Tax.NO.KidVariant.Mod11, new GenerationOptions { Seed = seed }, 2, buffer, out int written);
            if (generated) Veritas.Tax.NO.Kid.TryValidate(buffer[..written], Veritas.Tax.NO.KidVariant.Mod11, out _).ShouldBeTrue();
            else written.ShouldBe(0);
        }
    }

    [Fact]
    public void Mrz_EncodesExpiryYearInsteadOfCentury()
    {
        Span<char> buffer = stackalloc char[89];
        for (int seed = 0; seed < 50; seed++)
        {
            IcaoMrz.TryGenerate(new GenerationOptions { Seed = seed }, buffer, out int written).ShouldBeTrue();
            int expiryYear = (buffer[66] - '0') * 10 + buffer[67] - '0';
            expiryYear.ShouldBeInRange(25, 35);
            IcaoMrz.TryValidate(buffer[..written], out _).ShouldBeTrue();
        }
    }

    [Theory]
    [InlineData("00.000.000/E08G-12", true)]
    [InlineData("00000000E08G13", false)]
    [InlineData("04252011000110", true)]
    [InlineData("S0000000000018", true)]
    [InlineData("s0000000000018", true)]
    [InlineData("ſ0000000000018", false)]
    public void Cnpj_SupportsAlphanumericAndLegacyFormats(string input, bool expected)
        => Veritas.Tax.BR.Cnpj.TryValidate(input, out _).ShouldBe(expected);

    [Theory]
    [InlineData("150600A905P", true)]
    [InlineData("150600B905P", true)]
    [InlineData("150600Y905P", true)]
    [InlineData("150600F905P", true)]
    [InlineData("000000-0000", false)]
    public void Hetu_SupportsNewCenturyMarkersAndRealDates(string input, bool expected)
        => Veritas.Tax.FI.Hetu.TryValidate(input, out _).ShouldBe(expected);

    [Theory]
    [InlineData("7ZZZZZZZZZZZZZZZZZZZZZZZZZ", true)]
    [InlineData("80000000000000000000000000", false)]
    [InlineData("ZZZZZZZZZZZZZZZZZZZZZZZZZZ", false)]
    public void Ulid_RejectsOverflow(string input, bool expected)
        => Ulid.TryValidate(input, out _).ShouldBe(expected);

    [Theory]
    [InlineData("aWgEPTl1tmebfsQzFP4bxwgy80V", true)]
    [InlineData("aWgEPTl1tmebfsQzFP4bxwgy80W", false)]
    [InlineData("zzzzzzzzzzzzzzzzzzzzzzzzzzz", false)]
    public void Ksuid_RejectsOverflow(string input, bool expected)
        => Ksuid.TryValidate(input, out _).ShouldBe(expected);

    [Fact]
    public void Ksuid_ExplicitSeedIsReproducible()
    {
        var options = new GenerationOptions { Seed = 17 };
        Span<char> first = stackalloc char[27];
        Span<char> second = stackalloc char[27];
        Ksuid.TryGenerate(options, first, out _).ShouldBeTrue();
        Ksuid.TryGenerate(options, second, out _).ShouldBeTrue();
        first.SequenceEqual(second).ShouldBeTrue();
    }

    [Theory]
    [InlineData("zh-Hant-TW", true)]
    [InlineData("es-419", true)]
    [InlineData("de-CH-1901", true)]
    [InlineData("en-US-u-ca-gregory", true)]
    [InlineData("x-private", true)]
    [InlineData("i-klingon", true)]
    [InlineData("en-a-foo-a-bar", false)]
    [InlineData("de-1901-1901", false)]
    [InlineData("en_US", false)]
    [InlineData("en-x", false)]
    public void Bcp47_ImplementsWellFormedSyntax(string input, bool expected)
        => Bcp47.TryValidate(input, out _).ShouldBe(expected);
}
