using Veritas.Healthcare;
using Xunit;
using Shouldly;

public class IcdCodeTests
{
    [Theory]
    [InlineData("A01", true)]
    [InlineData("A01.1", true)]
    [InlineData("123", true)]
    [InlineData("123.4", true)]
    [InlineData("A0", false)]
    [InlineData("AA1", false)]
    public void Validate(string input, bool expected)
    {
        IcdCode.TryValidate(input, out var result).ShouldBe(expected);
        result.IsValid.ShouldBe(expected);
    }

    [Fact]
    public void GenerateProducesValid()
    {
        Span<char> buffer = stackalloc char[10];
        IcdCode.TryGenerate(buffer, out var written).ShouldBeTrue();
        IcdCode.TryValidate(buffer[..written], out var result).ShouldBeTrue();
        result.IsValid.ShouldBeTrue();
    }
}

