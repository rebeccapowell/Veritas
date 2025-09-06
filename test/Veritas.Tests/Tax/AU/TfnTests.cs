using Veritas.Tax.AU;
using Xunit;
using Shouldly;

public class TfnTests
{
    [Theory]
    [InlineData("123456782", true)]
    [InlineData("123456789", false)]
    public void Validate_Works(string input, bool expected)
    {
        Tfn.TryValidate(input, out var result);
        result.IsValid.ShouldBe(expected);
    }

    [Fact]
    public void GenerateProducesValid()
    {
        Span<char> buffer = stackalloc char[9];
        Tfn.TryGenerate(buffer, out var written).ShouldBeTrue();
        Tfn.TryValidate(buffer[..written], out var result);
        result.IsValid.ShouldBeTrue();
    }
}
