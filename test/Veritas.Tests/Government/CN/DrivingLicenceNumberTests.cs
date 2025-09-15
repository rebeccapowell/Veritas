using Veritas.Government.CN;
using Xunit;
using Shouldly;

public class ChinaDrivingLicenceNumberTests
{
    [Theory]
    [InlineData("123456789012", true)]
    [InlineData("000000000000", true)]
    [InlineData("12345678901", false)]
    [InlineData("12345678901A", false)]
    public void Validate(string input, bool expected)
    {
        DrivingLicenceNumber.TryValidate(input, out var result).ShouldBe(expected);
        result.IsValid.ShouldBe(expected);
    }

    [Fact]
    public void GenerateProducesValid()
    {
        Span<char> buffer = stackalloc char[12];
        DrivingLicenceNumber.TryGenerate(buffer, out var written).ShouldBeTrue();
        DrivingLicenceNumber.TryValidate(buffer[..written], out var result).ShouldBeTrue();
        result.IsValid.ShouldBeTrue();
    }
}

