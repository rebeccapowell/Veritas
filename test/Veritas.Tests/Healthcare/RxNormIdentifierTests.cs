using Veritas.Healthcare;
using Xunit;
using Shouldly;

public class RxNormIdentifierTests
{
    [Theory]
    [InlineData("123456", true)]
    [InlineData("1", true)]
    [InlineData("123456789", false)]
    [InlineData("ABC", false)]
    public void Validate(string input, bool expected)
    {
        RxNormIdentifier.TryValidate(input, out var result).ShouldBe(expected);
        result.IsValid.ShouldBe(expected);
    }

    [Fact]
    public void GenerateProducesValid()
    {
        Span<char> buffer = stackalloc char[8];
        RxNormIdentifier.TryGenerate(buffer, out var written).ShouldBeTrue();
        RxNormIdentifier.TryValidate(buffer[..written], out var result).ShouldBeTrue();
        result.IsValid.ShouldBeTrue();
    }
}

