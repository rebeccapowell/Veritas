using AutoFixture.Xunit2;
using Shouldly;
using Veritas.Tax.NL;
using Xunit;

public class BsnTests
{
    [Theory]
    [InlineData("100000009", true)]
    [InlineData("100000008", false)]
    public void Validate(string input, bool expected)
    {
        Bsn.TryValidate(input, out var result).ShouldBe(expected);
        result.IsValid.ShouldBe(expected);
    }

    [Theory, AutoData]
    public void RandomString_IsInvalid(string random)
    {
        Bsn.TryValidate(random, out var result).ShouldBeFalse();
        result.IsValid.ShouldBeFalse();
    }
}

