using System;
using Veritas.Government;
using Xunit;
using Shouldly;

public class VisaNumberTests
{
    [Theory]
    [InlineData("A1234567", true)]
    [InlineData("12345678", true)]
    [InlineData("ABCD1234", true)]
    [InlineData("1234567", false)]
    [InlineData("123456789", false)]
    [InlineData("12$45678", false)]
    public void Validate(string input, bool expected)
    {
        VisaNumber.TryValidate(input, out var result).ShouldBe(expected);
        result.IsValid.ShouldBe(expected);
    }

    [Fact]
    public void GenerateProducesValid()
    {
        Span<char> buffer = stackalloc char[8];
        VisaNumber.TryGenerate(buffer, out var written).ShouldBeTrue();
        VisaNumber.TryValidate(buffer[..written], out var result).ShouldBeTrue();
        result.IsValid.ShouldBeTrue();
    }
}

