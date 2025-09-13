using System;
using Veritas.Tax.AR;
using Xunit;
using Shouldly;

public class CuitTests
{
    [Theory]
    [InlineData("20-12345678-6", true)]
    [InlineData("20-12345678-5", false)]
    public void Validate_Works(string input, bool expected)
    {
        Cuit.TryValidate(input, out var result).ShouldBe(expected);
        result.IsValid.ShouldBe(expected);
    }

    [Fact]
    public void GenerateProducesValid()
    {
        Span<char> buffer = stackalloc char[11];
        Cuit.TryGenerate(buffer, out var written).ShouldBeTrue();
        Cuit.TryValidate(buffer[..written], out var result).ShouldBeTrue();
        result.IsValid.ShouldBeTrue();
    }
}

