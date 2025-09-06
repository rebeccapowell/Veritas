using AutoFixture.Xunit2;
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
        Piva.TryValidate(input, out var result);
        result.IsValid.ShouldBe(expected);
    }

    [Theory, AutoData]
    public void RandomString_IsInvalid(string random)
    {
        Piva.TryValidate(random, out var result);
        result.IsValid.ShouldBeFalse();
    }
}

