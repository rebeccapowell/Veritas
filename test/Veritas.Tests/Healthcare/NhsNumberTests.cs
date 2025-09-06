using Veritas.Healthcare;
using Xunit;
using Shouldly;

public class NhsNumberTests
{
    [Theory]
    [InlineData("9434765919", true)]
    [InlineData("9434765918", false)]
    public void Validate(string input, bool expected)
    {
        NhsNumber.TryValidate(input, out var r);
        r.IsValid.ShouldBe(expected);
    }

    [Fact]
    public void GenerateProducesValid()
    {
        Span<char> buffer = stackalloc char[10];
        NhsNumber.TryGenerate(buffer, out var written).ShouldBeTrue();
        NhsNumber.TryValidate(buffer[..written], out var r);
        r.IsValid.ShouldBeTrue();
    }
}
