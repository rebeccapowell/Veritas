using System;
using Veritas.Logistics;
using Xunit;
using Shouldly;

public class GlnTests
{
    [Theory]
    [InlineData("1234567890128", true)]
    [InlineData("1234567890123", false)]
    public void Validate(string input, bool expected)
    {
        Gln.TryValidate(input, out var r);
        r.IsValid.ShouldBe(expected);
    }

    [Fact]
    public void GenerateProducesValid()
    {
        Span<char> buffer = stackalloc char[13];
        Gln.TryGenerate(buffer, out var written).ShouldBeTrue();
        Gln.TryValidate(buffer[..written], out var r);
        r.IsValid.ShouldBeTrue();
    }
}
