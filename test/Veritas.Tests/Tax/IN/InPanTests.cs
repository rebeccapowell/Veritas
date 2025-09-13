using Veritas.Tax.IN;
using Xunit;
using Shouldly;

public class InPanTests
{
    [Theory]
    [InlineData("ABCDE1234I", true)]
    [InlineData("ABCDE1234A", false)]
    public void Validate_Works(string input, bool expected)
    {
        Pan.TryValidate(input, out var result).ShouldBe(expected);
        result.IsValid.ShouldBe(expected);
    }

    [Fact]
    public void GenerateProducesValid()
    {
        Span<char> buffer = stackalloc char[10];
        Pan.TryGenerate(buffer, out var written).ShouldBeTrue();
        Pan.TryValidate(buffer[..written], out var result).ShouldBeTrue();
        result.IsValid.ShouldBeTrue();
    }
}
