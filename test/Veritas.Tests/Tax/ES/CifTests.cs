using Shouldly;
using Veritas.Tax.ES;
using Xunit;

public class CifTests
{
    [Theory]
    [InlineData("A58818501", true)]
    [InlineData("B12345678", false)]
    public void Validate(string input, bool expected)
    {
        Cif.TryValidate(input, out var result).ShouldBe(expected);
        result.IsValid.ShouldBe(expected);
    }

    [Theory]
    [InlineData("invalid-identifier-5b43cc2c-6aee-4db4-ae60-58c47888d1cf")]
    [InlineData("")]
    [InlineData("\u0661\u0662\u0663")]
    public void MalformedString_IsInvalid(string random)
    {
        Cif.TryValidate(random, out var result).ShouldBeFalse();
        result.IsValid.ShouldBeFalse();
    }
}

