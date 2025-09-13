using Veritas.Tax;
using Xunit;
using Shouldly;

public class EoriTests
{
    [Theory]
    [InlineData("DE123456789012345", true)]
    [InlineData("fr12345678901234", true)]
    [InlineData("F123", false)]
    [InlineData("DE1234567890123456", false)]
    public void Validate(string input, bool expected)
    {
        Eori.TryValidate(input, out var r).ShouldBe(expected);
        r.IsValid.ShouldBe(expected);
    }

    [Fact]
    public void GenerateProducesValid()
    {
        Span<char> buffer = stackalloc char[17];
        Eori.TryGenerate(buffer, out var written).ShouldBeTrue();
        Eori.TryValidate(buffer[..written], out var r).ShouldBeTrue();
        r.IsValid.ShouldBeTrue();
    }
}
