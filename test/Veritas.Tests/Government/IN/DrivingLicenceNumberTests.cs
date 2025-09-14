using Veritas.Government.IN;
using Xunit;
using Shouldly;

public class IndiaDrivingLicenceNumberTests
{
    [Theory]
    [InlineData("KA01-20150012345", true)]
    [InlineData("DL05-19990000001", true)]
    [InlineData("K001-20150012345", false)]
    [InlineData("KA0120150012345", true)]
    public void Validate(string input, bool expected)
    {
        DrivingLicenceNumber.TryValidate(input, out var result).ShouldBe(expected);
        result.IsValid.ShouldBe(expected);
    }

    [Fact]
    public void GenerateProducesValid()
    {
        Span<char> buffer = stackalloc char[15];
        DrivingLicenceNumber.TryGenerate(buffer, out var written).ShouldBeTrue();
        DrivingLicenceNumber.TryValidate(buffer[..written], out var result).ShouldBeTrue();
        result.IsValid.ShouldBeTrue();
    }
}

