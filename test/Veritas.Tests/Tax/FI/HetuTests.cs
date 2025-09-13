using System;
using Veritas.Tax.FI;
using Xunit;
using Shouldly;

public class HetuTests
{
    [Theory]
    [InlineData("131052-308T", true)]
    [InlineData("131052-308U", false)]
    public void Validate_Works(string input, bool expected)
    {
        Hetu.TryValidate(input, out var result);
        result.IsValid.ShouldBe(expected);
    }

    [Fact]
    public void GenerateProducesValid()
    {
        Span<char> buffer = stackalloc char[11];
        Hetu.TryGenerate(buffer, out var written).ShouldBeTrue();
        Hetu.TryValidate(buffer[..written], out var result);
        result.IsValid.ShouldBeTrue();
    }
}

