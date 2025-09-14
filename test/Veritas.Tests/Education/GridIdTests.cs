using System;
using Veritas.Education;
using Xunit;
using Shouldly;

public class GridIdTests
{
    [Theory]
    [InlineData("GRID.1234567.A", true)]
    [InlineData("GRID.7654321.1", true)]
    [InlineData("GRID1234567A", false)]
    [InlineData("GRID.123456.A", false)]
    [InlineData("GRID.1234567.$", false)]
    public void Validate(string input, bool expected)
    {
        GridId.TryValidate(input, out var result).ShouldBe(expected);
        result.IsValid.ShouldBe(expected);
    }

    [Fact]
    public void GenerateProducesValid()
    {
        Span<char> buffer = stackalloc char[14];
        GridId.TryGenerate(buffer, out var written).ShouldBeTrue();
        GridId.TryValidate(buffer[..written], out var result).ShouldBeTrue();
        result.IsValid.ShouldBeTrue();
    }
}
