using System;
using Veritas.Algorithms;
using Xunit;
using Shouldly;

public class Iso7064Tests
{
    [Fact]
    public void ComputeCheckDigitsMod97_10_ProducesExpected()
    {
        var digits = "3214282912345698765432161100".AsSpan();
        var check = Iso7064.ComputeCheckDigitsMod97_10(digits);
        check.ShouldBe(82);
    }

    [Theory]
    [InlineData("3214282912345698765432161182", true)]
    [InlineData("3214282912345698765432161183", false)]
    public void ValidateMod97_10_Works(string input, bool expected)
    {
        var result = Iso7064.ValidateMod97_10(input);
        result.ShouldBe(expected);
    }

    [Fact]
    public void ComputeMod97_ReturnsRemainder()
    {
        var digits = "3214282912345698765432161182".AsSpan();
        var rem = Iso7064.ComputeMod97(digits);
        rem.ShouldBe(1);
    }

    [Fact]
    public void ComputeCheckDigitMod11_10_Works()
    {
        var cd = Iso7064.ComputeCheckDigitMod11_10("79462".AsSpan());
        cd.ShouldBe('3');
    }

    [Theory]
    [InlineData("794623", true)]
    [InlineData("794624", false)]
    public void ValidateMod11_10_Works(string input, bool expected)
    {
        var ok = Iso7064.ValidateMod11_10(input);
        ok.ShouldBe(expected);
    }

    [Fact]
    public void Mod37_2_Roundtrip()
    {
        var baseStr = "XAT000001234567";
        var check = Iso7064.ComputeCheckCharacterMod37_2(baseStr);
        var full = baseStr + check;
        Iso7064.ValidateMod37_2(full).ShouldBeTrue();
        Iso7064.ValidateMod37_2(baseStr + (check == '0' ? '1' : '0')).ShouldBeFalse();
    }

    [Theory]
    [InlineData("ABC", 'Z')]
    [InlineData("123456", 'J')]
    public void ComputeCheckCharacterMod37_36_Works(string input, char expected)
    {
        var check = Iso7064.ComputeCheckCharacterMod37_36(input.AsSpan());
        check.ShouldBe(expected);
    }

    [Theory]
    [InlineData("ABCZ", true)]
    [InlineData("123456J", true)]
    [InlineData("ABC0", false)]
    [InlineData("", false)]
    public void ValidateMod37_36_Works(string input, bool expected)
    {
        Iso7064.ValidateMod37_36(input).ShouldBe(expected);
    }
}
