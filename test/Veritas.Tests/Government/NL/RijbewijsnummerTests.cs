using Veritas.Government.NL;
using Xunit;
using Shouldly;

public class RijbewijsnummerTests
{
    [Theory]
    [InlineData("AB123456C", true)]
    [InlineData("ZX987654K", true)]
    [InlineData("A123456BC", false)]
    [InlineData("AB12345C", false)]
    public void Validate(string input, bool expected)
    {
        Rijbewijsnummer.TryValidate(input, out var result).ShouldBe(expected);
        result.IsValid.ShouldBe(expected);
    }

    [Fact]
    public void GenerateProducesValid()
    {
        Span<char> buffer = stackalloc char[9];
        Rijbewijsnummer.TryGenerate(buffer, out var written).ShouldBeTrue();
        Rijbewijsnummer.TryValidate(buffer[..written], out var result).ShouldBeTrue();
        result.IsValid.ShouldBeTrue();
    }
}

