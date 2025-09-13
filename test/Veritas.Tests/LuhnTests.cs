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

    [Theory]
    [InlineData("ABCDEFU", true)]
    [InlineData("ABCDEFX", false)]
    [InlineData("ABC#DEF", false)]
    [InlineData("", false)]
    public void ValidateBase36_Works(string input, bool expected)
    {
        Luhn.ValidateBase36(input).ShouldBe(expected);
    }

    [Fact]
    public void ComputeCheckCharacterBase36_ProducesExpected()
    {
        var check = Luhn.ComputeCheckCharacterBase36("ABCDEF".AsSpan());
        check.ShouldBe('U');
    }

    [Fact]
    public void Base36_RoundTripRandom()
    {
        var rng = new Random(123);
        Span<char> buf = stackalloc char[32];
        Span<char> total = stackalloc char[33];
        for (int i = 0; i < 100; i++)
        {
            int len = rng.Next(1, 20);
            for (int j = 0; j < len; j++)
            {
                int v = rng.Next(36);
                buf[j] = v < 10 ? (char)('0' + v) : (char)('A' + v - 10);
            }
            var check = Luhn.ComputeCheckCharacterBase36(buf[..len]);
            buf[..len].CopyTo(total);
            total[len] = check;
            Luhn.ValidateBase36(total[..(len + 1)]).ShouldBeTrue();
        }
    }
}
