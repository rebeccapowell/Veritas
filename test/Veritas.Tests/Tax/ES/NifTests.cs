using AutoFixture.Xunit2;
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

    [Theory, AutoData]
    public void RandomString_IsInvalid(string random)
    {
        Nif.TryValidate(random, out var result).ShouldBeFalse();
        result.IsValid.ShouldBeFalse();
    }
}

