using System;
using Veritas.Tax.BE;
using Xunit;
using Shouldly;

public class NnTests
{
    [Theory]
    [InlineData("90010100123", true)]
    [InlineData("90010100124", false)]
    public void Validate_Works(string input, bool expected)
    {
        Nn.TryValidate(input, out var result);
        result.IsValid.ShouldBe(expected);
    }

    [Fact]
    public void GenerateProducesValid()
    {
        Span<char> buffer = stackalloc char[11];
        Nn.TryGenerate(buffer, out var written).ShouldBeTrue();
        Nn.TryValidate(buffer[..written], out var result);
        result.IsValid.ShouldBeTrue();
    }
}

