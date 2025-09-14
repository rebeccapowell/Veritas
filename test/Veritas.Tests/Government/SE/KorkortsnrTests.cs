using Veritas.Government.SE;
using Xunit;
using Shouldly;

public class KorkortsnrTests
{
    [Theory]
    [InlineData("198507099805", true)]
    [InlineData("200001019876", true)]
    [InlineData("8507099805", false)]
    [InlineData("19850709980A", false)]
    public void Validate(string input, bool expected)
    {
        Korkortsnr.TryValidate(input, out var result).ShouldBe(expected);
        result.IsValid.ShouldBe(expected);
    }

    [Fact]
    public void GenerateProducesValid()
    {
        Span<char> buffer = stackalloc char[12];
        Korkortsnr.TryGenerate(buffer, out var written).ShouldBeTrue();
        Korkortsnr.TryValidate(buffer[..written], out var result).ShouldBeTrue();
        result.IsValid.ShouldBeTrue();
    }
}

