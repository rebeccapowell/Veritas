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

    [Theory]
    [InlineData("invalid-identifier-5b43cc2c-6aee-4db4-ae60-58c47888d1cf")]
    [InlineData("")]
    [InlineData("\u0661\u0662\u0663")]
    public void MalformedString_IsInvalid(string random)
    {
        Vat.TryValidate(random, out var result).ShouldBeFalse();
        result.IsValid.ShouldBeFalse();
    }
}

