using System;
using Veritas.Tax.DE;
using Xunit;
using Shouldly;

public class IdNrTests
{
    [Theory]
    [InlineData("36574261809", true)]
    [InlineData("36574261890", false)]
    public void Validate(string input, bool expected)
    {
        IdNr.TryValidate(input, out var result);
        result.IsValid.ShouldBe(expected);
    }

    [Fact]
    public void GenerateProducesValid()
    {
        Span<char> buffer = stackalloc char[11];
        IdNr.TryGenerate(buffer, out var written).ShouldBeTrue();
        IdNr.TryValidate(buffer[..written], out var result);
        result.IsValid.ShouldBeTrue();
    }
}
