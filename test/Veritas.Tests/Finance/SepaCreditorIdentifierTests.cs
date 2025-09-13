using System;
using Veritas.Finance;
using Xunit;
using Shouldly;

public class SepaCreditorIdentifierTests
{
    [Theory]
    [InlineData("DE74ZZZ09999999999", true)]
    [InlineData("DE75ZZZ09999999999", false)]
    public void Validate_Works(string input, bool expected)
    {
        SepaCreditorIdentifier.TryValidate(input, out var result).ShouldBe(expected);
        result.IsValid.ShouldBe(expected);
    }

    [Fact]
    public void GenerateProducesValid()
    {
        Span<char> buffer = stackalloc char[35];
        SepaCreditorIdentifier.TryGenerate(buffer, out var written).ShouldBeTrue();
        SepaCreditorIdentifier.TryValidate(buffer[..written], out var result).ShouldBeTrue();
        result.IsValid.ShouldBeTrue();
    }
}
