using System;
using Veritas.Energy.ES;
using Xunit;
using Shouldly;

public class CupsTests
{
    [Theory]
    [InlineData("ES1234123456789012JY", true)]
    [InlineData("ES1234123456789012XY", false)]
    [InlineData("ES1234123456789012JY1F", true)]
    [InlineData("ES1234123456789012JY1Q", false)]
    public void Validate(string input, bool expected)
    {
        Cups.TryValidate(input, out var result).ShouldBe(expected);
        result.IsValid.ShouldBe(expected);
    }

    [Fact]
    public void GenerateProducesValid()
    {
        Span<char> buffer = stackalloc char[20];
        Cups.TryGenerate(buffer, out var written).ShouldBeTrue();
        Cups.TryValidate(buffer[..written], out var result).ShouldBeTrue();
        result.IsValid.ShouldBeTrue();
    }
}
