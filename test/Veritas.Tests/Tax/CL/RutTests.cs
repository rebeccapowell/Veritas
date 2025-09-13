using System;
using Veritas.Tax.CL;
using Xunit;
using Shouldly;

public class RutTests
{
    [Theory]
    [InlineData("12.345.678-5", true)]
    [InlineData("12.345.678-4", false)]
    public void Validate_Works(string input, bool expected)
    {
        Rut.TryValidate(input, out var result);
        result.IsValid.ShouldBe(expected);
    }

    [Fact]
    public void GenerateProducesValid()
    {
        Span<char> buffer = stackalloc char[9];
        Rut.TryGenerate(buffer, out var written).ShouldBeTrue();
        Rut.TryValidate(buffer[..written], out var result);
        result.IsValid.ShouldBeTrue();
    }
}

