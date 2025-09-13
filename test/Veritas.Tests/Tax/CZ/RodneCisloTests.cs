using Veritas.Tax.CZ;
using Xunit;
using Shouldly;

public class CzRodneCisloTests
{
    [Theory]
    [InlineData("850712/1238", true)]
    [InlineData("850712/1239", false)]
    public void Validate_Works(string input, bool expected)
    {
        RodneCislo.TryValidate(input, out var result);
        result.IsValid.ShouldBe(expected);
    }

    [Fact]
    public void GenerateProducesValid()
    {
        Span<char> buffer = stackalloc char[10];
        RodneCislo.TryGenerate(buffer, out var written).ShouldBeTrue();
        RodneCislo.TryValidate(buffer[..written], out var result);
        result.IsValid.ShouldBeTrue();
    }
}
