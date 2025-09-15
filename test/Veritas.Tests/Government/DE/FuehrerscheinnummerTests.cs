using Veritas.Government.DE;
using Xunit;
using Shouldly;

public class FuehrerscheinnummerTests
{
    [Theory]
    [InlineData("ABC1234567", true)]
    [InlineData("1A2B3C4D5E", true)]
    [InlineData("AB12", false)]
    [InlineData("ABCD!23456", false)]
    public void Validate(string input, bool expected)
    {
        Fuehrerscheinnummer.TryValidate(input, out var result).ShouldBe(expected);
        result.IsValid.ShouldBe(expected);
    }

    [Fact]
    public void GenerateProducesValid()
    {
        Span<char> buffer = stackalloc char[12];
        Fuehrerscheinnummer.TryGenerate(buffer, out var written).ShouldBeTrue();
        Fuehrerscheinnummer.TryValidate(buffer[..written], out var result).ShouldBeTrue();
        result.IsValid.ShouldBeTrue();
    }
}

