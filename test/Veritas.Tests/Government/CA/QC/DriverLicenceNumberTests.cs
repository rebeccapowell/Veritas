using Veritas.Government.CA.QC;
using Xunit;
using Shouldly;

public class QuebecDriverLicenceNumberTests
{
    [Theory]
    [InlineData("A123456789012", true)]
    [InlineData("Z000000000000", true)]
    [InlineData("1234567890123", false)]
    [InlineData("AA23456789012", false)]
    public void Validate(string input, bool expected)
    {
        DriverLicenceNumber.TryValidate(input, out var result).ShouldBe(expected);
        result.IsValid.ShouldBe(expected);
    }

    [Fact]
    public void GenerateProducesValid()
    {
        Span<char> buffer = stackalloc char[13];
        DriverLicenceNumber.TryGenerate(buffer, out var written).ShouldBeTrue();
        DriverLicenceNumber.TryValidate(buffer[..written], out var result).ShouldBeTrue();
        result.IsValid.ShouldBeTrue();
    }
}

