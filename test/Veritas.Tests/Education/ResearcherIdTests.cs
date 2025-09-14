using System;
using Veritas.Education;
using Xunit;
using Shouldly;

public class ResearcherIdTests
{
    [Theory]
    [InlineData("A-1234-2008", true)]
    [InlineData("B-0001-1999", true)]
    [InlineData("a-1234-2008", false)]
    [InlineData("A1234-2008", false)]
    [InlineData("A-1234-1899", false)]
    public void Validate(string input, bool expected)
    {
        ResearcherId.TryValidate(input, out var result).ShouldBe(expected);
        result.IsValid.ShouldBe(expected);
    }

    [Fact]
    public void GenerateProducesValid()
    {
        Span<char> buffer = stackalloc char[11];
        ResearcherId.TryGenerate(buffer, out var written).ShouldBeTrue();
        ResearcherId.TryValidate(buffer[..written], out var result).ShouldBeTrue();
        result.IsValid.ShouldBeTrue();
    }
}
