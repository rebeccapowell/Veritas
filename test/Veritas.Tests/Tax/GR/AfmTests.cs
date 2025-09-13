using System;
using Veritas.Tax.GR;
using Xunit;
using Shouldly;

public class AfmTests
{
    [Theory]
    [InlineData("291417771", true)]
    [InlineData("291417770", false)]
    public void Validate_Works(string input, bool expected)
    {
        Afm.TryValidate(input, out var result).ShouldBe(expected);
        result.IsValid.ShouldBe(expected);
    }

    [Fact]
    public void GenerateProducesValid()
    {
        Span<char> buffer = stackalloc char[9];
        Afm.TryGenerate(buffer, out var written).ShouldBeTrue();
        Afm.TryValidate(buffer[..written], out var result).ShouldBeTrue();
        result.IsValid.ShouldBeTrue();
    }
}

