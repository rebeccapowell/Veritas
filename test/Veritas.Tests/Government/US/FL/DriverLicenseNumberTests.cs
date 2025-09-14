using Veritas.Government.US.FL;
using Xunit;
using Shouldly;

public class FloridaDriverLicenseNumberTests
{
    [Theory]
    [InlineData("F123456789012", true)]
    [InlineData("A000000000000", true)]
    [InlineData("1234567890123", false)]
    [InlineData("FF23456789012", false)]
    public void Validate(string input, bool expected)
    {
        DriverLicenseNumber.TryValidate(input, out var result).ShouldBe(expected);
        result.IsValid.ShouldBe(expected);
    }

    [Fact]
    public void GenerateProducesValid()
    {
        Span<char> buffer = stackalloc char[13];
        DriverLicenseNumber.TryGenerate(buffer, out var written).ShouldBeTrue();
        DriverLicenseNumber.TryValidate(buffer[..written], out var result).ShouldBeTrue();
        result.IsValid.ShouldBeTrue();
    }
}

