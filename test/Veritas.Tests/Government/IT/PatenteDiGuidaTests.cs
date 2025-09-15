using Veritas.Government.IT;
using Xunit;
using Shouldly;

public class PatenteDiGuidaTests
{
    [Theory]
    [InlineData("AB123CDE45", true)]
    [InlineData("0000000000", true)]
    [InlineData("AB123", false)]
    [InlineData("AB123CDE4!", false)]
    public void Validate(string input, bool expected)
    {
        PatenteDiGuida.TryValidate(input, out var result).ShouldBe(expected);
        result.IsValid.ShouldBe(expected);
    }

    [Fact]
    public void GenerateProducesValid()
    {
        Span<char> buffer = stackalloc char[10];
        PatenteDiGuida.TryGenerate(buffer, out var written).ShouldBeTrue();
        PatenteDiGuida.TryValidate(buffer[..written], out var result).ShouldBeTrue();
        result.IsValid.ShouldBeTrue();
    }
}

