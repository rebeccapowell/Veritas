using Veritas.Healthcare;
using Xunit;
using Shouldly;

public class DeaNumberTests
{
    [Theory]
    [InlineData("AB1234563", true)]
    [InlineData("AB1234560", false)]
    [InlineData("A91234563", true)]
    public void Validate(string input, bool expected)
    {
        DeaNumber.TryValidate(input, out var r).ShouldBe(expected);
        r.IsValid.ShouldBe(expected);
    }

    [Fact]
    public void GenerateProducesValid()
    {
        Span<char> buffer = stackalloc char[9];
        DeaNumber.TryGenerate(buffer, out var written).ShouldBeTrue();
        DeaNumber.TryValidate(buffer[..written], out var r).ShouldBeTrue();
        r.IsValid.ShouldBeTrue();
    }
}
