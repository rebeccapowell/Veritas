using Veritas.Tax.NO;
using Xunit;
using Shouldly;

public class NoKidTests
{
    [Theory]
    [InlineData("12344", KidVariant.Mod10, true)]
    [InlineData("12345", KidVariant.Mod10, false)]
    [InlineData("12343", KidVariant.Mod11, true)]
    [InlineData("12344", KidVariant.Mod11, false)]
    public void Validate_Works(string input, KidVariant variant, bool expected)
    {
        Kid.TryValidate(input, variant, out var result);
        result.IsValid.ShouldBe(expected);
    }

    [Fact]
    public void Generate_Mod10_Valid()
    {
        Span<char> buffer = stackalloc char[5];
        Kid.TryGenerate(KidVariant.Mod10, 5, buffer, out var written).ShouldBeTrue();
        Kid.TryValidate(buffer[..written], KidVariant.Mod10, out var result);
        result.IsValid.ShouldBeTrue();
    }

    [Fact]
    public void Generate_Mod11_Valid()
    {
        Span<char> buffer = stackalloc char[5];
        Kid.TryGenerate(KidVariant.Mod11, 5, buffer, out var written).ShouldBeTrue();
        Kid.TryValidate(buffer[..written], KidVariant.Mod11, out var result);
        result.IsValid.ShouldBeTrue();
    }
}
