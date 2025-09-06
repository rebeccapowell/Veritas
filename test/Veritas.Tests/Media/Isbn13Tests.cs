using System;
using Veritas.Media;
using Xunit;
using Shouldly;

public class Isbn13Tests
{
    [Theory]
    [InlineData("9780306406157", true)]
    [InlineData("9780306406158", false)]
    public void Validate(string input, bool expected)
    {
        Isbn13.TryValidate(input, out var r);
        r.IsValid.ShouldBe(expected);
    }

    [Fact]
    public void GenerateProducesValid()
    {
        Span<char> buffer = stackalloc char[13];
        Isbn13.TryGenerate(buffer, out var written).ShouldBeTrue();
        Isbn13.TryValidate(buffer[..written], out var r);
        r.IsValid.ShouldBeTrue();
    }
}
