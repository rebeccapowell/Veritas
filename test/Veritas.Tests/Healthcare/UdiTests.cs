using Veritas.Healthcare;
using Xunit;
using Shouldly;

public class UdiTests
{
    [Theory]
    [InlineData("(01)12345678901234(21)ABC123", true)]
    [InlineData("+A123B456C789", true)]
    [InlineData("=ABC12345", true)]
    [InlineData("12345", false)]
    public void Validate(string input, bool expected)
    {
        Udi.TryValidate(input, out var result).ShouldBe(expected);
        result.IsValid.ShouldBe(expected);
    }

    [Fact]
    public void GenerateProducesValid()
    {
        Span<char> buffer = stackalloc char[32];
        Udi.TryGenerate(buffer, out var written).ShouldBeTrue();
        Udi.TryValidate(buffer[..written], out var result).ShouldBeTrue();
        result.IsValid.ShouldBeTrue();
    }
}

