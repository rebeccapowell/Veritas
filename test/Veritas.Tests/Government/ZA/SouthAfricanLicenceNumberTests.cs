using Veritas.Government.ZA;
using Xunit;
using Shouldly;

public class SouthAfricanLicenceNumberTests
{
    [Theory]
    [InlineData("ABC", false)]
    [InlineData("123", false)]
    public void Validate(string input, bool expected)
    {
        SouthAfricanLicenceNumber.TryValidate(input, out var result).ShouldBe(expected);
        result.IsValid.ShouldBe(expected);
    }

    [Fact]
    public void GenerateProducesValid()
    {
        Span<char> buffer = stackalloc char[13];
        SouthAfricanLicenceNumber.TryGenerate(buffer, out var written).ShouldBeTrue();
        SouthAfricanLicenceNumber.TryValidate(buffer[..written], out var result).ShouldBeTrue();
        result.IsValid.ShouldBeTrue();
    }
}

