using Veritas.Government.NZ;
using Xunit;
using Shouldly;

public class NzDriverLicenceNumberTests
{
    [Theory]
    [InlineData("AB1234CD", true)]
    [InlineData("12345678", true)]
    [InlineData("ABC1234", false)]
    [InlineData("AB1234C!", false)]
    public void Validate(string input, bool expected)
    {
        DriverLicenceNumber.TryValidate(input, out var result).ShouldBe(expected);
        result.IsValid.ShouldBe(expected);
    }

    [Fact]
    public void GenerateProducesValid()
    {
        Span<char> buffer = stackalloc char[8];
        DriverLicenceNumber.TryGenerate(buffer, out var written).ShouldBeTrue();
        DriverLicenceNumber.TryValidate(buffer[..written], out var result).ShouldBeTrue();
        result.IsValid.ShouldBeTrue();
    }
}

