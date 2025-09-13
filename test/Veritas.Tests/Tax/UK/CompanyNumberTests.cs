using AutoFixture.Xunit2;
using Shouldly;
using Veritas.Tax.UK;
using Xunit;

public class CompanyNumberTests
{
    [Theory]
    [InlineData("01234567", true)]
    [InlineData("SC123456", true)]
    [InlineData("1234567", false)]
    public void Validate(string input, bool expected)
    {
        CompanyNumber.TryValidate(input, out var result).ShouldBe(expected);
        result.IsValid.ShouldBe(expected);
    }

    [Theory, AutoData]
    public void RandomString_IsInvalid(string random)
    {
        CompanyNumber.TryValidate(random, out var result).ShouldBeFalse();
        result.IsValid.ShouldBeFalse();
    }
}

