using System;
using Shouldly;
using Xunit;

public class ChecksumRegressionTests
{
    [Fact]
    public void Eic_UnrestrictedGenerationIsUnavailable()
    {
        Veritas.Energy.Eic.TryGenerate(new char[16], out int written).ShouldBeFalse();
        written.ShouldBe(0);
    }

    [Fact]
    public void Sepa_DefaultGenerationUsesGermanNationalFormat()
    {
        Span<char> buffer = stackalloc char[35];
        for (int seed = 0; seed < 100; seed++)
        {
            Veritas.Finance.SepaCreditorIdentifier.TryGenerate(new Veritas.GenerationOptions { Seed = seed }, buffer, out int written).ShouldBeTrue();
            written.ShouldBe(18);
            new string(buffer[..2]).ShouldBe("DE");
            buffer[7].ShouldBe('0');
            Veritas.Finance.SepaCreditorIdentifier.TryValidate(buffer[..written], out _).ShouldBeTrue();
        }
    }

    [Theory]
    [InlineData("11Y123456789012T", true)]
    [InlineData("11XRWENET12345-2", true)]
    [InlineData("11Y1234567890126", false)]
    [InlineData("XAT000001234567Y", false)]
    public void Eic_UsesEntsoeVectors(string input, bool expected)
        => Veritas.Energy.Eic.TryValidate(input, out _).ShouldBe(expected);

    [Theory]
    [InlineData("1200000000002", true)]
    [InlineData("1200000000006", false)]
    [InlineData("1234567890126", true)]
    public void Mpan_UsesPrimeWeights(string input, bool expected)
        => Veritas.Energy.GB.Mpan.TryValidate(input, out _).ShouldBe(expected);

    [Theory]
    [InlineData("DE98ZZZ09999999999", true)]
    [InlineData("DE98ABC09999999999", true)]
    [InlineData("DE74ZZZ09999999999", false)]
    [InlineData("DE01ZZZ09999999999", false)]
    public void Sepa_ExcludesBusinessCode(string input, bool expected)
        => Veritas.Finance.SepaCreditorIdentifier.TryValidate(input, out _).ShouldBe(expected);

    [Theory]
    [InlineData("BBG000BLNNH6", true)]
    [InlineData("KKG000000M81", true)]
    [InlineData("BBG000BLNNH7", false)]
    [InlineData("000000000000", false)]
    public void Figi_UsesCharacterWeightsAndStructure(string input, bool expected)
        => Veritas.Finance.Figi.TryValidate(input, out _).ShouldBe(expected);

    [Theory]
    [InlineData("G4594H109", true)]
    [InlineData("G4594H102", false)]
    [InlineData("037833100", true)]
    public void Cusip_SumsWeightedDecimalDigits(string input, bool expected)
        => Veritas.Finance.Cusip.TryValidate(input, out _).ShouldBe(expected);

    [Theory]
    [InlineData("RSSMRA85T10A562S", true)]
    [InlineData("RSSMRA85T10A562G", false)]
    [InlineData("RSSMRA85B31A562F", false)]
    [InlineData("RSSMRA85T10A56UD", true)]
    [InlineData("123MRA85T10A562P", false)]
    public void CodiceFiscale_UsesCorrectEvenPositionMapping(string input, bool expected)
        => Veritas.Tax.IT.CodiceFiscale.TryValidate(input, out _).ShouldBe(expected);
}
