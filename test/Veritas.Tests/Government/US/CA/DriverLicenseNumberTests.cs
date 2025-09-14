using Veritas.Government.US.CA;
using Xunit;
using Shouldly;

public class CaliforniaDriverLicenseNumberTests
{
    [Theory]
    [InlineData("A1234567", true)]
    [InlineData("Z7654321", true)]
    [InlineData("12345678", false)]
    [InlineData("AA123456", false)]
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

