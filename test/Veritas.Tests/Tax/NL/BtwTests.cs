using AutoFixture.Xunit2;
using Shouldly;
using Veritas.Tax.NL;
using Xunit;

public class BtwTests
{
    [Theory]
    [InlineData("100000002B01", true)]
    [InlineData("100000003B01", false)]
    public void Validate(string input, bool expected)
    {
        Btw.TryValidate(input, out var result).ShouldBe(expected);
        result.IsValid.ShouldBe(expected);
    }

    [Theory, AutoData]
    public void RandomString_IsInvalid(string random)
    {
        Btw.TryValidate(random, out var result).ShouldBeFalse();
        result.IsValid.ShouldBeFalse();
    }
}

