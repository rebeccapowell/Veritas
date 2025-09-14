using Veritas.Geospatial.DE;

public class FlurstuecknummerTests
{
    [Theory]
    [InlineData("1234 567 8901", true)]
    [InlineData("1234 567 8901/0002", true)]
    [InlineData("1234567890", false)]
    [InlineData("12a4 567 8901", false)]
    public void Validate(string input, bool expected)
    {
        Flurstuecknummer.TryValidate(input, out var result).ShouldBe(expected);
        result.IsValid.ShouldBe(expected);
    }

    [Fact]
    public void GenerateProducesValid()
    {
        Span<char> buffer = stackalloc char[20];
        Flurstuecknummer.TryGenerate(buffer, out var written).ShouldBeTrue();
        Flurstuecknummer.TryValidate(buffer[..written], out var result).ShouldBeTrue();
        result.IsValid.ShouldBeTrue();
    }
}
