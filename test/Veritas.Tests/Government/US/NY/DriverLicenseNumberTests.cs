using Veritas.Government.US.NY;
using Xunit;
using Shouldly;

public class NewYorkDriverLicenseNumberTests
{
    [Theory]
    [InlineData("123456789", true)]
    [InlineData("000000000", true)]
    [InlineData("A23456789", false)]
    [InlineData("12345678", false)]
    public void Validate(string input, bool expected)
    {
        DriverLicenseNumber.TryValidate(input, out var result).ShouldBe(expected);
        result.IsValid.ShouldBe(expected);
    }

    [Fact]
    public void GenerateProducesValid()
    {
        Span<char> buffer = stackalloc char[9];
        DriverLicenseNumber.TryGenerate(buffer, out var written).ShouldBeTrue();
        DriverLicenseNumber.TryValidate(buffer[..written], out var result).ShouldBeTrue();
        result.IsValid.ShouldBeTrue();
    }
}

