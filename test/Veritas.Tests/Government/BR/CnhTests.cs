using Veritas.Government.BR;
using Xunit;
using Shouldly;

public class CnhTests
{
    [Theory]
    [InlineData("ABCDEFGHIJK", false)]
    [InlineData("1234567890", false)]
    public void Validate(string input, bool expected)
    {
        Cnh.TryValidate(input, out var result).ShouldBe(expected);
        result.IsValid.ShouldBe(expected);
    }

    [Fact]
    public void GenerateProducesValid()
    {
        Span<char> buffer = stackalloc char[11];
        Cnh.TryGenerate(buffer, out var written).ShouldBeTrue();
        Cnh.TryValidate(buffer[..written], out var result).ShouldBeTrue();
        result.IsValid.ShouldBeTrue();
    }
}

