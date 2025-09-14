using System;
using Veritas.Government;
using Xunit;
using Shouldly;

public class PassportNumberTests
{
    [Theory]
    [InlineData("A123456", true)]
    [InlineData("1234567", true)]
    [InlineData("AB 123456", true)]
    [InlineData("A1", false)]
    [InlineData("1234567890", false)]
    [InlineData("ABC$123", false)]
    public void Validate(string input, bool expected)
    {
        PassportNumber.TryValidate(input, out var result).ShouldBe(expected);
        result.IsValid.ShouldBe(expected);
    }

    [Fact]
    public void GenerateProducesValid()
    {
        Span<char> buffer = stackalloc char[10];
        PassportNumber.TryGenerate(buffer, out var written).ShouldBeTrue();
        PassportNumber.TryValidate(buffer[..written], out var result).ShouldBeTrue();
        result.IsValid.ShouldBeTrue();
    }
}

