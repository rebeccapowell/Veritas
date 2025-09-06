using Veritas.Finance;
using Xunit;
using Shouldly;

public class SedolTests
{
    [Theory]
    [InlineData("B0YBKJ7", true)]
    [InlineData("B0YBKJ8", false)]
    public void Validate_Works(string input, bool expected)
    {
        Sedol.TryValidate(input, out var result);
        result.IsValid.ShouldBe(expected);
    }

    [Fact]
    public void GenerateProducesValid()
    {
        Span<char> buffer = stackalloc char[7];
        Sedol.TryGenerate(buffer, out var written).ShouldBeTrue();
        Sedol.TryValidate(buffer[..written], out var result);
        result.IsValid.ShouldBeTrue();
    }
}
