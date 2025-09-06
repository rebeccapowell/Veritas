using System;
using Veritas.Energy.NL;
using Xunit;
using Shouldly;

public class EnergyEanTests
{
    [Theory]
    [InlineData("123456789012345675", true)]
    [InlineData("123456789012345674", false)]
    [InlineData("12345678901234567", false)]
    public void Validate(string input, bool expected)
    {
        EnergyEan.TryValidate(input, out var result);
        result.IsValid.ShouldBe(expected);
    }

    [Fact]
    public void GenerateProducesValid()
    {
        Span<char> buffer = stackalloc char[18];
        EnergyEan.TryGenerate(buffer, out var written).ShouldBeTrue();
        EnergyEan.TryValidate(buffer[..written], out var result);
        result.IsValid.ShouldBeTrue();
    }
}
