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
        int[] weights = {5,4,3,2,1};
        Mod11.ComputeMod11(digits, weights).ShouldBe(2);
    }
}

