using AutoFixture.Xunit2;
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

    [Theory, AutoData]
    public void RandomString_IsInvalid(string random)
    {
        Cif.TryValidate(random, out var result).ShouldBeFalse();
        result.IsValid.ShouldBeFalse();
    }
}

