using Veritas.Healthcare;
using Xunit;
using Shouldly;

public class NdcPackageCodeTests
{
    [Theory]
    [InlineData("12345-678-90-12", true)]
    [InlineData("123456789012", true)]
    [InlineData("12345678901", false)]
    [InlineData("ABCDEFGHIJKL", false)]
    public void Validate(string input, bool expected)
    {
        NdcPackageCode.TryValidate(input, out var result).ShouldBe(expected);
        result.IsValid.ShouldBe(expected);
    }

    [Fact]
    public void GenerateProducesValid()
    {
        Span<char> buffer = stackalloc char[16];
        NdcPackageCode.TryGenerate(buffer, out var written).ShouldBeTrue();
        NdcPackageCode.TryValidate(buffer[..written], out var result).ShouldBeTrue();
        result.IsValid.ShouldBeTrue();
    }
}

