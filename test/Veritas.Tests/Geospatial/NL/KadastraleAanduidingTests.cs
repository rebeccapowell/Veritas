using Veritas.Geospatial.NL;

public class KadastraleAanduidingTests
{
    [Theory]
    [InlineData("ABCD A 1234", true)]
    [InlineData("abcd a 1", true)]
    [InlineData("ABC A 1234", false)]
    [InlineData("ABCD 1 1234", false)]
    public void Validate(string input, bool expected)
    {
        KadastraleAanduiding.TryValidate(input, out var result).ShouldBe(expected);
        result.IsValid.ShouldBe(expected);
    }

    [Fact]
    public void GenerateProducesValid()
    {
        Span<char> buffer = stackalloc char[16];
        KadastraleAanduiding.TryGenerate(buffer, out var written).ShouldBeTrue();
        KadastraleAanduiding.TryValidate(buffer[..written], out var result).ShouldBeTrue();
        result.IsValid.ShouldBeTrue();
    }
}
