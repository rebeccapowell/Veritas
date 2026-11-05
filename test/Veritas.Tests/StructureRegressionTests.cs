using System;
using Shouldly;
using Xunit;

public class StructureRegressionTests
{
    [Fact]
    public void Sepa_BulgarianTypeTwoRequiresNinePayloadCharacters()
        => Veritas.Finance.SepaCreditorIdentifier.TryValidate("BG08ZZZ2AAAAAAAA", out _).ShouldBeFalse();

    [Theory]
    [InlineData("DE091", false)]
    [InlineData("ZZ391", false)]
    [InlineData("DE89370400440532013000", true)]
    [InlineData("GB82WEST12345698765432", true)]
    [InlineData("FR1420041010050500013M02606", true)]
    public void Iban_RequiresRegisteredCountryFormat(string input, bool expected)
        => Veritas.Finance.Iban.TryValidate(input, out _).ShouldBe(expected);

    [Fact]
    public void Isin_RejectsNumericCountryPrefix()
        => Veritas.Finance.Isin.TryValidate("000000000000", out _).ShouldBeFalse();

    [Fact]
    public void Malo_RejectsZeroInitialDigit()
        => Veritas.Energy.DE.Malo.TryValidate("00000000000", out _).ShouldBeFalse();

    [Fact]
    public void Sepa_RejectsUnknownCountryAndWrongGermanLength()
    {
        Veritas.Finance.SepaCreditorIdentifier.TryValidate("ZZ09ZZZ1", out _).ShouldBeFalse();
        Veritas.Finance.SepaCreditorIdentifier.TryValidate("DE87ZZZ1", out _).ShouldBeFalse();
    }
}
