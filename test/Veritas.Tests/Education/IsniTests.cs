using System;
using Veritas.Education;
using Xunit;
using Shouldly;

public class IsniTests
{
    [Theory]
    [InlineData("0000 0001 2146 438X", true)]
    [InlineData("000000012146438X", true)]
    [InlineData("0000000121464380", false)]
    [InlineData("000000012146438", false)]
    public void Validate(string input, bool expected)
    {
        Isni.TryValidate(input, out var r).ShouldBe(expected);
        r.IsValid.ShouldBe(expected);
    }

    [Fact]
    public void GenerateProducesValid()
    {
        Span<char> buffer = stackalloc char[16];
        Isni.TryGenerate(buffer, out var written).ShouldBeTrue();
        Isni.TryValidate(buffer[..written], out var r).ShouldBeTrue();
        r.IsValid.ShouldBeTrue();
    }
}
