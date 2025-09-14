using Veritas.Government.US.TX;
using Xunit;
using Shouldly;

public class TexasDriverLicenseNumberTests
{
    [Theory]
    [InlineData("12345678", true)]
    [InlineData("00000000", true)]
    [InlineData("A2345678", false)]
    [InlineData("123456789", false)]
    public void Validate(string input, bool expected)
    {
        DriverLicenseNumber.TryValidate(input, out var result).ShouldBe(expected);
        result.IsValid.ShouldBe(expected);
    }

    [Fact]
    public void GenerateProducesValid()
    {
        Span<char> buffer = stackalloc char[8];
        DriverLicenseNumber.TryGenerate(buffer, out var written).ShouldBeTrue();
        DriverLicenseNumber.TryValidate(buffer[..written], out var result).ShouldBeTrue();
        result.IsValid.ShouldBeTrue();
    }
}

