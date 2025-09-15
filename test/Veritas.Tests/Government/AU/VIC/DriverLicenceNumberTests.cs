using Veritas.Government.AU.VIC;
using Xunit;
using Shouldly;

public class VicDriverLicenceNumberTests
{
    [Theory]
    [InlineData("1234567890", true)]
    [InlineData("0000000000", true)]
    [InlineData("123456789", false)]
    [InlineData("123456789A", false)]
    public void Validate(string input, bool expected)
    {
        DriverLicenceNumber.TryValidate(input, out var result).ShouldBe(expected);
        result.IsValid.ShouldBe(expected);
    }

    [Fact]
    public void GenerateProducesValid()
    {
        Span<char> buffer = stackalloc char[10];
        DriverLicenceNumber.TryGenerate(buffer, out var written).ShouldBeTrue();
        DriverLicenceNumber.TryValidate(buffer[..written], out var result).ShouldBeTrue();
        result.IsValid.ShouldBeTrue();
    }
}

