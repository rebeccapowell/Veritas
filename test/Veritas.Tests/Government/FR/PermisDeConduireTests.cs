using Veritas.Government.FR;
using Xunit;
using Shouldly;

public class PermisDeConduireTests
{
    [Theory]
    [InlineData("123456789012", true)]
    [InlineData("000000000000", true)]
    [InlineData("12345678901", false)]
    [InlineData("12345678901A", false)]
    public void Validate(string input, bool expected)
    {
        PermisDeConduire.TryValidate(input, out var result).ShouldBe(expected);
        result.IsValid.ShouldBe(expected);
    }

    [Fact]
    public void GenerateProducesValid()
    {
        Span<char> buffer = stackalloc char[12];
        PermisDeConduire.TryGenerate(buffer, out var written).ShouldBeTrue();
        PermisDeConduire.TryValidate(buffer[..written], out var result).ShouldBeTrue();
        result.IsValid.ShouldBeTrue();
    }
}

