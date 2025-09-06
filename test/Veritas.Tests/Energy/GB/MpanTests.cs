using System;
using Veritas.Energy.GB;
using Xunit;
using Shouldly;

public class MpanTests
{
    [Theory]
    [InlineData("1234567890123", true)]
    [InlineData("1234567890124", false)]
    public void Validate_Works(string input, bool expected)
    {
        Mpan.TryValidate(input, out var result);
        result.IsValid.ShouldBe(expected);
    }

    [Fact]
    public void GenerateProducesValid()
    {
        Span<char> buffer = stackalloc char[13];
        Mpan.TryGenerate(buffer, out var written).ShouldBeTrue();
        Mpan.TryValidate(buffer[..written], out var result);
        result.IsValid.ShouldBeTrue();
    }
}

