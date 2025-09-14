using Veritas.Government.AU.QLD;
using Xunit;
using Shouldly;

public class QldDriverLicenceNumberTests
{
    [Theory]
    [InlineData("123456789", true)]
    [InlineData("000000000", true)]
    [InlineData("12345678", false)]
    [InlineData("12345678A", false)]
    public void Validate(string input, bool expected)
    {
        DriverLicenceNumber.TryValidate(input, out var result).ShouldBe(expected);
        result.IsValid.ShouldBe(expected);
    }

    [Fact]
    public void GenerateProducesValid()
    {
        Span<char> buffer = stackalloc char[9];
        DriverLicenceNumber.TryGenerate(buffer, out var written).ShouldBeTrue();
        DriverLicenceNumber.TryValidate(buffer[..written], out var result).ShouldBeTrue();
        result.IsValid.ShouldBeTrue();
    }
}

