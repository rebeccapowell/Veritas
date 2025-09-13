using System;
using Veritas.Tax.PT;
using Xunit;
using Shouldly;

public class PtNifTests
{
    [Theory]
    [InlineData("166048763", true)]
    [InlineData("166048764", false)]
    public void Validate_Works(string input, bool expected)
    {
        Nif.TryValidate(input, out var result).ShouldBe(expected);
        result.IsValid.ShouldBe(expected);
    }

    [Fact]
    public void GenerateProducesValid()
    {
        Span<char> buffer = stackalloc char[9];
        Nif.TryGenerate(buffer, out var written).ShouldBeTrue();
        Nif.TryValidate(buffer[..written], out var result).ShouldBeTrue();
        result.IsValid.ShouldBeTrue();
    }
}

