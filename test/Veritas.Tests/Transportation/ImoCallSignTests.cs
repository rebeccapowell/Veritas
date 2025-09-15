using Veritas.Transportation;
using Xunit;
using Shouldly;

public class ImoCallSignTests
{
    [Theory]
    [InlineData("ABCD1", true)]
    [InlineData("AAA000", true)]
    [InlineData("AB", false)]
    [InlineData("ABCDEFGH", false)]
    [InlineData("A$1", false)]
    public void Validate(string input, bool expected)
    {
        ImoCallSign.TryValidate(input, out var result).ShouldBe(expected);
        result.IsValid.ShouldBe(expected);
    }

    [Fact]
    public void GenerateProducesValid()
    {
        Span<char> buffer = stackalloc char[7];
        ImoCallSign.TryGenerate(buffer, out var written).ShouldBeTrue();
        ImoCallSign.TryValidate(buffer[..written], out var result).ShouldBeTrue();
        result.IsValid.ShouldBeTrue();
    }
}
