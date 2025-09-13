using System;
using Veritas.Tax.IE;
using Xunit;
using Shouldly;

public class PpsnTests
{
    [Theory]
    [InlineData("1234567FA", true)]
    [InlineData("1234567WA", false)]
    public void Validate_Works(string input, bool expected)
    {
        Ppsn.TryValidate(input, out var result).ShouldBe(expected);
        result.IsValid.ShouldBe(expected);
    }

    [Fact]
    public void GenerateProducesValid()
    {
        Span<char> buffer = stackalloc char[9];
        Ppsn.TryGenerate(buffer, out var written).ShouldBeTrue();
        Ppsn.TryValidate(buffer[..written], out var result).ShouldBeTrue();
        result.IsValid.ShouldBeTrue();
    }
}

