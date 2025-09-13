using Veritas.Tax.US;
using Xunit;
using Shouldly;

public class SsnTests
{
    [Theory]
    [InlineData("123-45-6789", true)]
    [InlineData("000-00-0000", false)]
    public void Validate(string input, bool expected)
    {
        Ssn.TryValidate(input, out var r).ShouldBe(expected);
        r.IsValid.ShouldBe(expected);
    }

    [Fact]
    public void GenerateProducesValid()
    {
        Span<char> buffer = stackalloc char[9];
        Ssn.TryGenerate(buffer, out var written).ShouldBeTrue();
        Ssn.TryValidate(buffer[..written], out var r).ShouldBeTrue();
        r.IsValid.ShouldBeTrue();
    }
}
