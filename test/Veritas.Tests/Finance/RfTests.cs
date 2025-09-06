using System;
using Veritas.Finance;
using Xunit;
using Shouldly;

public class RfTests
{
    [Theory]
    [InlineData("RF18539007547034", true)]
    [InlineData("RF00539007547034", false)]
    public void Validate_Works(string input, bool expected)
    {
        Rf.TryValidate(input, out var result);
        result.IsValid.ShouldBe(expected);
    }

    [Fact]
    public void GenerateProducesValid()
    {
        Span<char> buffer = stackalloc char[25];
        Rf.TryGenerate(buffer, out var written).ShouldBeTrue();
        Rf.TryValidate(buffer[..written], out var result);
        result.IsValid.ShouldBeTrue();
    }
}

