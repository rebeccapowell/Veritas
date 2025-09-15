using System;
using Veritas.Standards;
using Xunit;
using Shouldly;

public class UlFileNumberTests
{
    [Theory]
    [InlineData("E123456", true)]
    [InlineData("e-000001", true)]
    [InlineData("E12345", false)]
    [InlineData("A123456", false)]
    public void Validate(string input, bool expected)
    {
        UlFileNumber.TryValidate(input, out var result).ShouldBe(expected);
        result.IsValid.ShouldBe(expected);
    }

    [Fact]
    public void GenerateProducesValid()
    {
        Span<char> buffer = stackalloc char[7];
        UlFileNumber.TryGenerate(buffer, out var written).ShouldBeTrue();
        UlFileNumber.TryValidate(buffer[..written], out var result).ShouldBeTrue();
        result.IsValid.ShouldBeTrue();
    }
}

