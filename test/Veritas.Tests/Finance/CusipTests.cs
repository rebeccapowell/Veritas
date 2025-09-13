using Veritas.Finance;
using Xunit;
using Shouldly;

public class CusipTests
{
    [Theory]
    [InlineData("037833100", true)]
    [InlineData("037833101", false)]
    public void Validate_Works(string input, bool expected)
    {
        Cusip.TryValidate(input, out var result).ShouldBe(expected);
        result.IsValid.ShouldBe(expected);
    }

    [Fact]
    public void GenerateProducesValid()
    {
        Span<char> buffer = stackalloc char[9];
        Cusip.TryGenerate(buffer, out var written).ShouldBeTrue();
        Cusip.TryValidate(buffer[..written], out var result).ShouldBeTrue();
        result.IsValid.ShouldBeTrue();
    }
}
