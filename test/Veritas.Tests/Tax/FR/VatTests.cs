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

