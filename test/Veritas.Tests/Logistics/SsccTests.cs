using System;
using Veritas.Logistics;
using Xunit;
using Shouldly;

public class SsccTests
{
    [Theory]
    [InlineData("123456789012345675", true)]
    [InlineData("123456789012345671", false)]
    public void Validate(string input, bool expected)
    {
        Sscc.TryValidate(input, out var r);
        r.IsValid.ShouldBe(expected);
    }

    [Fact]
    public void GenerateProducesValid()
    {
        Span<char> buffer = stackalloc char[18];
        Sscc.TryGenerate(buffer, out var written).ShouldBeTrue();
        Sscc.TryValidate(buffer[..written], out var r);
        r.IsValid.ShouldBeTrue();
    }
}
