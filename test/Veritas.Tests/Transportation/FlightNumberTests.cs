using Veritas.Transportation;
using Xunit;
using Shouldly;

public class FlightNumberTests
{
    [Theory]
    [InlineData("BA123", true)]
    [InlineData("AA1", true)]
    [InlineData("BA", false)]
    [InlineData("BAA123", false)]
    [InlineData("BA12A", false)]
    public void Validate(string input, bool expected)
    {
        FlightNumber.TryValidate(input, out var result).ShouldBe(expected);
        result.IsValid.ShouldBe(expected);
    }

    [Fact]
    public void GenerateProducesValid()
    {
        Span<char> buffer = stackalloc char[6];
        FlightNumber.TryGenerate(buffer, out var written).ShouldBeTrue();
        FlightNumber.TryValidate(buffer[..written], out var result).ShouldBeTrue();
        result.IsValid.ShouldBeTrue();
    }
}
