using System;
using Veritas.Tax.DE;
using Xunit;
using Shouldly;

public class UstIdNrTests
{
    [Theory]
    [InlineData("DE136695976", true)]
    [InlineData("136695976", true)]
    [InlineData("136695978", false)]
    public void Validate(string input, bool expected)
    {
        UstIdNr.TryValidate(input, out var result).ShouldBe(expected);
        result.IsValid.ShouldBe(expected);
    }

    [Fact]
    public void GenerateProducesValid()
    {
        Span<char> buffer = stackalloc char[9];
        UstIdNr.TryGenerate(buffer, out var written).ShouldBeTrue();
        UstIdNr.TryValidate(buffer[..written], out var result).ShouldBeTrue();
        result.IsValid.ShouldBeTrue();
    }
}
