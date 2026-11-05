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

    [Theory]
    [InlineData("invalid-identifier-5b43cc2c-6aee-4db4-ae60-58c47888d1cf")]
    [InlineData("")]
    [InlineData("\u0661\u0662\u0663")]
    public void MalformedString_IsInvalid(string random)
    {
        Btw.TryValidate(random, out var result).ShouldBeFalse();
        result.IsValid.ShouldBeFalse();
    }
}

