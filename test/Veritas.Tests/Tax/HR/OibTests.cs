using Veritas.Tax.HR;
using Xunit;
using Shouldly;

public class HrOibTests
{
    [Theory]
    [InlineData("12345678903", true)]
    [InlineData("12345678904", false)]
    public void Validate_Works(string input, bool expected)
    {
        Oib.TryValidate(input, out var result).ShouldBe(expected);
        result.IsValid.ShouldBe(expected);
    }

    [Fact]
    public void GenerateProducesValid()
    {
        Span<char> buffer = stackalloc char[11];
        Oib.TryGenerate(buffer, out var written).ShouldBeTrue();
        Oib.TryValidate(buffer[..written], out var result).ShouldBeTrue();
        result.IsValid.ShouldBeTrue();
    }
}
