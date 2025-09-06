using System;
using Veritas.Logistics;
using Xunit;
using Shouldly;

public class GtinTests
{
    [Theory]
    [InlineData("4006381333931", true)]
    [InlineData("4006381333932", false)]
    public void Validate(string input, bool expected)
    {
        Gtin.TryValidate(input, out var r);
        r.IsValid.ShouldBe(expected);
    }

    [Fact]
    public void GenerateProducesValid()
    {
        Span<char> buffer = stackalloc char[13];
        Gtin.TryGenerate(13, buffer, out var written).ShouldBeTrue();
        Gtin.TryValidate(buffer[..written], out var r);
        r.IsValid.ShouldBeTrue();
    }
}
