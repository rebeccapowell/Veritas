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

    [Theory]
    [InlineData("invalid-identifier-5b43cc2c-6aee-4db4-ae60-58c47888d1cf")]
    [InlineData("")]
    [InlineData("\u0661\u0662\u0663")]
    public void MalformedString_IsInvalid(string random)
    {
        CompanyNumber.TryValidate(random, out var result).ShouldBeFalse();
        result.IsValid.ShouldBeFalse();
    }
}

