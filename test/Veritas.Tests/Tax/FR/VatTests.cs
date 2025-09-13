using AutoFixture.Xunit2;
using Shouldly;
using Veritas.Tax.FR;
using Xunit;

public class FrVatTests
{
    [Theory]
    [InlineData("FR44 732829320", true)]
    [InlineData("FR45 732829320", false)]
    public void Validate(string input, bool expected)
    {
        Vat.TryValidate(input, out var result).ShouldBe(expected);
        result.IsValid.ShouldBe(expected);
    }

    [Theory, AutoData]
    public void RandomString_IsInvalid(string random)
    {
        Vat.TryValidate(random, out var result).ShouldBeFalse();
        result.IsValid.ShouldBeFalse();
    }
}

