using System;
using Veritas.Media;
using Xunit;
using Shouldly;

public class Isbn10Tests
{
    [Theory]
    [InlineData("0306406152", true)]
    [InlineData("0306406153", false)]
    public void Validate(string input, bool expected)
    {
        Isbn10.TryValidate(input, out var r).ShouldBe(expected);
        r.IsValid.ShouldBe(expected);
    }

    [Fact]
    public void GenerateProducesValid()
    {
        Span<char> buffer = stackalloc char[10];
        Isbn10.TryGenerate(buffer, out var written).ShouldBeTrue();
        Isbn10.TryValidate(buffer[..written], out var r).ShouldBeTrue();
        r.IsValid.ShouldBeTrue();
    }
}
