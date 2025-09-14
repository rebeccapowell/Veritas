using Veritas.Government.AR;
using Xunit;
using Shouldly;

public class LicenciaNacionalConducirTests
{
    [Theory]
    [InlineData("1234567", true)]
    [InlineData("12345678", true)]
    [InlineData("123456", false)]
    [InlineData("123456789", false)]
    public void Validate(string input, bool expected)
    {
        LicenciaNacionalConducir.TryValidate(input, out var result).ShouldBe(expected);
        result.IsValid.ShouldBe(expected);
    }

    [Fact]
    public void GenerateProducesValid()
    {
        Span<char> buffer = stackalloc char[8];
        LicenciaNacionalConducir.TryGenerate(buffer, out var written).ShouldBeTrue();
        LicenciaNacionalConducir.TryValidate(buffer[..written], out var result).ShouldBeTrue();
        result.IsValid.ShouldBeTrue();
    }
}

