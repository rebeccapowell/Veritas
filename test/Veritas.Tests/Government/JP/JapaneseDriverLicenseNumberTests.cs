using Veritas.Government.JP;
using Xunit;
using Shouldly;

public class JapaneseDriverLicenseNumberTests
{
    [Theory]
    [InlineData("123456789013", false)]
    [InlineData("ABCDEFGHIJKL", false)]
    public void Validate(string input, bool expected)
    {
        JapaneseDriverLicenseNumber.TryValidate(input, out var result).ShouldBe(expected);
        result.IsValid.ShouldBe(expected);
    }

    [Fact]
    public void GenerateProducesValid()
    {
        Span<char> buffer = stackalloc char[12];
        JapaneseDriverLicenseNumber.TryGenerate(buffer, out var written).ShouldBeTrue();
        JapaneseDriverLicenseNumber.TryValidate(buffer[..written], out var result).ShouldBeTrue();
        result.IsValid.ShouldBeTrue();
    }
}

