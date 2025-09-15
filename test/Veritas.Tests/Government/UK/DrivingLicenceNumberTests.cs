using Veritas.Government.UK;
using Xunit;
using Shouldly;

public class DrivingLicenceNumberTests
{
    [Theory]
    [InlineData("ABC", false)]
    [InlineData("SMITH801101AB9C$", false)]
    public void Validate(string input, bool expected)
    {
        DrivingLicenceNumber.TryValidate(input, out var result).ShouldBe(expected);
        result.IsValid.ShouldBe(expected);
    }

    [Fact]
    public void GenerateProducesValid()
    {
        Span<char> buffer = stackalloc char[16];
        DrivingLicenceNumber.TryGenerate(buffer, out var written).ShouldBeTrue();
        DrivingLicenceNumber.TryValidate(buffer[..written], out var result).ShouldBeTrue();
        result.IsValid.ShouldBeTrue();
    }
}

