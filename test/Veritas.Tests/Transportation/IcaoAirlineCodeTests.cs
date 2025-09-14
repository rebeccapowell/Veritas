using Veritas.Transportation;
using Xunit;
using Shouldly;

public class IcaoAirlineCodeTests
{
    [Theory]
    [InlineData("ABC", true)]
    [InlineData("BAW", true)]
    [InlineData("AB1", false)]
    [InlineData("AB", false)]
    [InlineData("ABCD", false)]
    public void Validate(string input, bool expected)
    {
        IcaoAirlineCode.TryValidate(input, out var result).ShouldBe(expected);
        result.IsValid.ShouldBe(expected);
    }

    [Fact]
    public void GenerateProducesValid()
    {
        Span<char> buffer = stackalloc char[3];
        IcaoAirlineCode.TryGenerate(buffer, out var written).ShouldBeTrue();
        IcaoAirlineCode.TryValidate(buffer[..written], out var result).ShouldBeTrue();
        result.IsValid.ShouldBeTrue();
    }
}
