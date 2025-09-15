using Veritas.Healthcare;
using Xunit;
using Shouldly;

public class NdcTests
{
    [Theory]
    [InlineData("12345-6789-0", true)]
    [InlineData("1234567890", true)]
    [InlineData("12345678901", true)]
    [InlineData("12345", false)]
    [InlineData("ABC", false)]
    public void Validate(string input, bool expected)
    {
        Ndc.TryValidate(input, out var result).ShouldBe(expected);
        result.IsValid.ShouldBe(expected);
    }

    [Fact]
    public void GenerateProducesValid()
    {
        Span<char> buffer = stackalloc char[16];
        Ndc.TryGenerate(buffer, out var written).ShouldBeTrue();
        Ndc.TryValidate(buffer[..written], out var result).ShouldBeTrue();
        result.IsValid.ShouldBeTrue();
    }
}

