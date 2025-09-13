using Veritas.Tax.NO;
using Xunit;
using Shouldly;

public class NoFodselsnummerTests
{
    [Theory]
    [InlineData("12078586468", true)]
    [InlineData("12078586469", false)]
    public void Validate_Works(string input, bool expected)
    {
        Fodselsnummer.TryValidate(input, out var result);
        result.IsValid.ShouldBe(expected);
    }

    [Fact]
    public void GenerateProducesValid()
    {
        Span<char> buffer = stackalloc char[11];
        Fodselsnummer.TryGenerate(buffer, out var written).ShouldBeTrue();
        Fodselsnummer.TryValidate(buffer[..written], out var result);
        result.IsValid.ShouldBeTrue();
    }
}
