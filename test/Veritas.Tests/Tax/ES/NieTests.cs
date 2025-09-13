using AutoFixture.Xunit2;
using Shouldly;
using Veritas.Tax.ES;
using Xunit;

public class NieTests
{
    [Theory]
    [InlineData("X2482300W", true)]
    [InlineData("X2482300A", false)]
    public void Validate(string input, bool expected)
    {
        Nie.TryValidate(input, out var result).ShouldBe(expected);
        result.IsValid.ShouldBe(expected);
    }

    [Theory, AutoData]
    public void RandomString_IsInvalid(string random)
    {
        Nie.TryValidate(random, out var result).ShouldBeFalse();
        result.IsValid.ShouldBeFalse();
    }
}

