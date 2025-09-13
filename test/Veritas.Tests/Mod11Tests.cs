using System;
using Veritas.Algorithms;
using Xunit;
using Shouldly;

public class Mod11Tests
{
    [Fact]
    public void ComputeMod11_Works()
    {
        var digits = "12345".AsSpan();
        int[] weights = { 5, 4, 3, 2, 1 };
        Mod11.ComputeMod11(digits, weights).ShouldBe(2);
    }

    [Fact]
    public void ComputeCheckCharacter_FromRightPattern()
    {
        int[] weights = { 2, 3, 4, 5, 6, 7 };
        var check = Mod11.ComputeCheckCharacter("123456789".AsSpan(), weights, fromRight: true);
        check.ShouldBe('2');
    }

    [Theory]
    [InlineData("1234567892", true)]
    [InlineData("1234567890", false)]
    public void Validate_FromRightPattern(string input, bool expected)
    {
        int[] weights = { 2, 3, 4, 5, 6, 7 };
        Mod11.Validate(input, weights, fromRight: true).ShouldBe(expected);
    }

    [Fact]
    public void ComputeCheckCharacter_IsbnExample()
    {
        int[] weights = { 10, 9, 8, 7, 6, 5, 4, 3, 2 };
        var check = Mod11.ComputeCheckCharacter("097522980".AsSpan(), weights);
        check.ShouldBe('X');
    }

    [Theory]
    [InlineData("097522980X", true)]
    [InlineData("0975229800", false)]
    [InlineData("", false)]
    [InlineData("09752298#X", false)]
    public void Validate_IsbnExample(string input, bool expected)
    {
        int[] weights = { 10, 9, 8, 7, 6, 5, 4, 3, 2 };
        Mod11.Validate(input, weights).ShouldBe(expected);
    }
}

