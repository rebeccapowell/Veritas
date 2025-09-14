using Veritas.Transportation;
using Xunit;
using Shouldly;

public class IataAirlineCodeTests
{
    [Theory]
    [InlineData("AA", true)]
    [InlineData("A1", true)]
    [InlineData("A", false)]
    [InlineData("AAA", false)]
    [InlineData("A$", false)]
    public void Validate(string input, bool expected)
    {
        IataAirlineCode.TryValidate(input, out var result).ShouldBe(expected);
        result.IsValid.ShouldBe(expected);
    }

    [Fact]
    public void GenerateProducesValid()
    {
        Span<char> buffer = stackalloc char[2];
        IataAirlineCode.TryGenerate(buffer, out var written).ShouldBeTrue();
        IataAirlineCode.TryValidate(buffer[..written], out var result).ShouldBeTrue();
        result.IsValid.ShouldBeTrue();
    }
}
