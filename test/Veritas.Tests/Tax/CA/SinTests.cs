using Veritas.Tax.CA;
using Xunit;
using Shouldly;

public class SinTests
{
    [Theory]
    [InlineData("046454286", true)]
    [InlineData("046454287", false)]
    public void Validate_Works(string input, bool expected)
    {
        Sin.TryValidate(input, out var result);
        result.IsValid.ShouldBe(expected);
    }

    [Fact]
    public void GenerateProducesValid()
    {
        Span<char> buffer = stackalloc char[9];
        Sin.TryGenerate(buffer, out var written).ShouldBeTrue();
        Sin.TryValidate(buffer[..written], out var result);
        result.IsValid.ShouldBeTrue();
    }
}
