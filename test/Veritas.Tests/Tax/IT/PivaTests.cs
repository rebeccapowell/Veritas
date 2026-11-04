using Shouldly;
using Veritas.Tax.IT;
using Xunit;

public class PivaTests
{
    [Theory]
    [InlineData("01114601006", true)]
    [InlineData("01114601007", false)]
    public void Validate(string input, bool expected)
    {
        Piva.TryValidate(input, out var result).ShouldBe(expected);
        result.IsValid.ShouldBe(expected);
    }

    [Theory]
    [InlineData("invalid-identifier-5b43cc2c-6aee-4db4-ae60-58c47888d1cf")]
    [InlineData("")]
    [InlineData("\u0661\u0662\u0663")]
    public void MalformedString_IsInvalid(string random)
    {
        Piva.TryValidate(random, out var result).ShouldBeFalse();
        result.IsValid.ShouldBeFalse();
    }
}

