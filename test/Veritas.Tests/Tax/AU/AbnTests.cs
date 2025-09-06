using Veritas.Tax.AU;
using Xunit;
using Shouldly;

public class AbnTests
{
    [Theory]
    [InlineData("51824753556", true)]
    [InlineData("51824753557", false)]
    public void Validate_Works(string input, bool expected)
    {
        Abn.TryValidate(input, out var result);
        result.IsValid.ShouldBe(expected);
    }

    [Fact]
    public void GenerateProducesValid()
    {
        Span<char> buffer = stackalloc char[11];
        Abn.TryGenerate(buffer, out var written).ShouldBeTrue();
        Abn.TryValidate(buffer[..written], out var result);
        result.IsValid.ShouldBeTrue();
    }
}
