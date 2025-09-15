using Veritas.Geospatial;

public class PlusCodeTests
{
    [Theory]
    [InlineData("849VCWC8+R9", true)]
    [InlineData("849V CWC8+R9", true)]
    [InlineData("849VCWC8R9", false)]
    [InlineData("849VCWC8+R@", false)]
    public void Validate(string input, bool expected)
    {
        PlusCode.TryValidate(input, out var result).ShouldBe(expected);
        result.IsValid.ShouldBe(expected);
    }

    [Fact]
    public void GenerateProducesValid()
    {
        Span<char> buffer = stackalloc char[16];
        PlusCode.TryGenerate(buffer, out var written).ShouldBeTrue();
        PlusCode.TryValidate(buffer[..written], out var result).ShouldBeTrue();
        result.IsValid.ShouldBeTrue();
    }
}
