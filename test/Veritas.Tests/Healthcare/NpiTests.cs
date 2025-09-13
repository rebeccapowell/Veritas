using Veritas.Healthcare;
using Xunit;
using Shouldly;

public class NpiTests
{
    [Theory]
    [InlineData("1234567893", true)]
    [InlineData("1234567890", false)]
    public void Validate(string input, bool expected)
    {
        Npi.TryValidate(input, out var r).ShouldBe(expected);
        r.IsValid.ShouldBe(expected);
    }

    [Fact]
    public void GenerateProducesValid()
    {
        Span<char> buffer = stackalloc char[10];
        Npi.TryGenerate(buffer, out var written).ShouldBeTrue();
        Npi.TryValidate(buffer[..written], out var r).ShouldBeTrue();
        r.IsValid.ShouldBeTrue();
    }
}
