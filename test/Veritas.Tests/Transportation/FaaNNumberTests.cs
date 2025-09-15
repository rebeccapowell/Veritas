using Veritas.Transportation;
using Xunit;
using Shouldly;

public class FaaNNumberTests
{
    [Theory]
    [InlineData("N1", true)]
    [InlineData("N1234Z", true)]
    [InlineData("N123AB", true)]
    [InlineData("N0123", false)]
    [InlineData("12345", false)]
    [InlineData("N12345A", false)]
    [InlineData("N12I", false)]
    public void Validate(string input, bool expected)
    {
        FaaNNumber.TryValidate(input, out var result).ShouldBe(expected);
        result.IsValid.ShouldBe(expected);
    }

    [Fact]
    public void GenerateProducesValid()
    {
        Span<char> buffer = stackalloc char[8];
        FaaNNumber.TryGenerate(buffer, out var written).ShouldBeTrue();
        FaaNNumber.TryValidate(buffer[..written], out var result).ShouldBeTrue();
        result.IsValid.ShouldBeTrue();
    }
}
