using AutoFixture.Xunit2;
using Shouldly;
using Veritas.Tax.UK;
using Xunit;

public class UkVatTests
{
    [Theory]
    [InlineData("GB980780684", true)]
    [InlineData("980780684", true)]
    [InlineData("980780685", false)]
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

