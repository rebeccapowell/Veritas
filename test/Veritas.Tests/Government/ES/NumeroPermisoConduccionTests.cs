using Veritas.Government.ES;
using Xunit;
using Shouldly;

public class NumeroPermisoConduccionTests
{
    [Theory]
    [InlineData("12345678Z", true)]
    [InlineData("00000000T", true)]
    [InlineData("12345678A", false)]
    [InlineData("1234567Z", false)]
    public void Validate(string input, bool expected)
    {
        NumeroPermisoConduccion.TryValidate(input, out var result).ShouldBe(expected);
        result.IsValid.ShouldBe(expected);
    }

    [Fact]
    public void GenerateProducesValid()
    {
        Span<char> buffer = stackalloc char[9];
        NumeroPermisoConduccion.TryGenerate(buffer, out var written).ShouldBeTrue();
        NumeroPermisoConduccion.TryValidate(buffer[..written], out var result).ShouldBeTrue();
        result.IsValid.ShouldBeTrue();
    }
}

