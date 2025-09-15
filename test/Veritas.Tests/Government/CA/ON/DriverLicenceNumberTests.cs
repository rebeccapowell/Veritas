using Veritas.Government.CA.ON;
using Xunit;
using Shouldly;

public class OntarioDriverLicenceNumberTests
{
    [Theory]
    [InlineData("A12345678901234", true)]
    [InlineData("Z00000000000000", true)]
    [InlineData("123456789012345", false)]
    [InlineData("AA2345678901234", false)]
    public void Validate(string input, bool expected)
    {
        DriverLicenceNumber.TryValidate(input, out var result).ShouldBe(expected);
        result.IsValid.ShouldBe(expected);
    }

    [Fact]
    public void GenerateProducesValid()
    {
        Span<char> buffer = stackalloc char[15];
        DriverLicenceNumber.TryGenerate(buffer, out var written).ShouldBeTrue();
        DriverLicenceNumber.TryValidate(buffer[..written], out var result).ShouldBeTrue();
        result.IsValid.ShouldBeTrue();
    }
}

