using System;
using Veritas.Algorithms;
using Xunit;
using Shouldly;

public class LuhnTests
{
    [Theory]
    [InlineData("79927398713", true)] // known valid
    [InlineData("79927398714", false)] // invalid
    [InlineData("490154203237518", true)] // IMEI example
    public void Validate_Works(string input, bool expected)
    {
        var result = Luhn.Validate(input);
        result.ShouldBe(expected);
    }

    [Fact]
    public void ComputeCheckDigit_ProducesExpected()
    {
        var digits = "7992739871".AsSpan();
        var check = Luhn.ComputeCheckDigit(digits);
        check.ShouldBe(3);
    }
}
