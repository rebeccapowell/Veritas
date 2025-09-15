using System;
using Veritas;
using Veritas.Finance;
using Xunit;
using Shouldly;

public class FigiTests
{
    [Theory]
    [InlineData("BBG000BLNNH0", false)]
    [InlineData("BBG00BLNNH6", false)]
    public void Validate_Invalid(string input, bool expected)
    {
        Figi.TryValidate(input, out var result).ShouldBe(expected);
        result.IsValid.ShouldBe(expected);
    }

    [Fact]
    public void GenerateProducesValid()
    {
        Span<char> buffer = stackalloc char[12];
        Figi.TryGenerate(new GenerationOptions { Seed = 123 }, buffer, out var written).ShouldBeTrue();
        Figi.TryValidate(buffer[..written], out var result).ShouldBeTrue();
        result.IsValid.ShouldBeTrue();
    }
}

