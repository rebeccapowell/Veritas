using System;
using Veritas.Education;
using Xunit;
using Shouldly;

public class RorIdTests
{
    [Theory]
    [InlineData("https://ror.org/01ggx4157", true)]
    [InlineData("https://ror.org/05hj2st04", true)]
    [InlineData("http://ror.org/05hj2st04", false)]
    [InlineData("https://ror.org/ABC123XYZ", false)]
    [InlineData("https://ror.org/123", false)]
    public void Validate(string input, bool expected)
    {
        RorId.TryValidate(input, out var result).ShouldBe(expected);
        result.IsValid.ShouldBe(expected);
    }

    [Fact]
    public void GenerateProducesValid()
    {
        Span<char> buffer = stackalloc char[25];
        RorId.TryGenerate(buffer, out var written).ShouldBeTrue();
        RorId.TryValidate(buffer[..written], out var result).ShouldBeTrue();
        result.IsValid.ShouldBeTrue();
    }
}
