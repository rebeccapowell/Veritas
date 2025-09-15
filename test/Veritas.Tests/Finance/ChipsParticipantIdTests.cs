using System;
using Veritas;
using Veritas.Finance;
using Xunit;
using Shouldly;

public class ChipsParticipantIdTests
{
    [Theory]
    [InlineData("0001", true)]
    [InlineData("9999", true)]
    [InlineData("12", false)]
    [InlineData("12345", false)]
    [InlineData("12A4", false)]
    public void Validate(string input, bool expected)
    {
        ChipsParticipantId.TryValidate(input, out var result).ShouldBe(expected);
        result.IsValid.ShouldBe(expected);
    }

    [Fact]
    public void GenerateProducesValid()
    {
        Span<char> buffer = stackalloc char[4];
        ChipsParticipantId.TryGenerate(new GenerationOptions { Seed = 123 }, buffer, out var written).ShouldBeTrue();
        ChipsParticipantId.TryValidate(buffer[..written], out var result).ShouldBeTrue();
        result.IsValid.ShouldBeTrue();
    }
}

