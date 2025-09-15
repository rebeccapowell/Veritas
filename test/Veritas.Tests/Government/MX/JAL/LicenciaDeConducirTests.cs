using Veritas.Government.MX.JAL;
using Xunit;
using Shouldly;

public class JalLicenciaDeConducirTests
{
    [Theory]
    [InlineData("12345678", true)]
    [InlineData("00000000", true)]
    [InlineData("1234567", false)]
    [InlineData("1234567A", false)]
    public void Validate(string input, bool expected)
    {
        LicenciaDeConducir.TryValidate(input, out var result).ShouldBe(expected);
        result.IsValid.ShouldBe(expected);
    }

    [Fact]
    public void GenerateProducesValid()
    {
        Span<char> buffer = stackalloc char[8];
        LicenciaDeConducir.TryGenerate(buffer, out var written).ShouldBeTrue();
        LicenciaDeConducir.TryValidate(buffer[..written], out var result).ShouldBeTrue();
        result.IsValid.ShouldBeTrue();
    }
}

