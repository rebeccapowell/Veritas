using Veritas.Geospatial;

public class GeohashTests
{
    [Theory]
    [InlineData("u4pruydqqvj", true)]
    [InlineData("u4pruydq qvj", true)]
    [InlineData("u4pruydqqvjpp", false)]
    [InlineData("!@#$", false)]
    public void Validate(string input, bool expected)
    {
        Geohash.TryValidate(input, out var result).ShouldBe(expected);
        result.IsValid.ShouldBe(expected);
    }

    [Fact]
    public void GenerateProducesValid()
    {
        Span<char> buffer = stackalloc char[12];
        Geohash.TryGenerate(buffer, out var written).ShouldBeTrue();
        Geohash.TryValidate(buffer[..written], out var result).ShouldBeTrue();
        result.IsValid.ShouldBeTrue();
    }
}
