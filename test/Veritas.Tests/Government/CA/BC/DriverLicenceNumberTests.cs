using Veritas.Government.CA.BC;
using Xunit;
using Shouldly;

public class BcDriverLicenceNumberTests
{
    [Theory]
    [InlineData("1234567", true)]
    [InlineData("0000000", true)]
    [InlineData("123456", false)]
    [InlineData("12345A7", false)]
    public void Validate(string input, bool expected)
    {
        DriverLicenceNumber.TryValidate(input, out var result).ShouldBe(expected);
        result.IsValid.ShouldBe(expected);
    }

    [Fact]
    public void GenerateProducesValid()
    {
        Span<char> buffer = stackalloc char[7];
        DriverLicenceNumber.TryGenerate(buffer, out var written).ShouldBeTrue();
        DriverLicenceNumber.TryValidate(buffer[..written], out var result).ShouldBeTrue();
        result.IsValid.ShouldBeTrue();
    }
}

