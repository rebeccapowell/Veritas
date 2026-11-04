using Shouldly;
using Veritas.Tax.ES;
using Xunit;

public class NifTests
{
    [Theory]
    [InlineData("12345678Z", true)]
    [InlineData("12345678A", false)]
    public void Validate(string input, bool expected)
    {
        Nif.TryValidate(input, out var result).ShouldBe(expected);
        result.IsValid.ShouldBe(expected);
    }

    [Theory]
    [InlineData("invalid-identifier-5b43cc2c-6aee-4db4-ae60-58c47888d1cf")]
    [InlineData("")]
    [InlineData("\u0661\u0662\u0663")]
    public void MalformedString_IsInvalid(string random)
    {
        Nif.TryValidate(random, out var result).ShouldBeFalse();
        result.IsValid.ShouldBeFalse();
    }
}

