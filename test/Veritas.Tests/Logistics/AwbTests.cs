using System;
using Veritas.Logistics;
using Xunit;
using Shouldly;

public class AwbTests
{
    [Theory]
    [InlineData("123-12345675", true)]
    [InlineData("12312345675", true)]
    [InlineData("123-12345674", false)]
    [InlineData("1231234567", false)]
    public void Validate(string input, bool expected)
    {
        Awb.TryValidate(input, out var r).ShouldBe(expected);
        r.IsValid.ShouldBe(expected);
    }

    [Fact]
    public void GenerateProducesValid()
    {
        Span<char> buffer = stackalloc char[11];
        Awb.TryGenerate(buffer, out var written).ShouldBeTrue();
        Awb.TryValidate(buffer[..written], out var r).ShouldBeTrue();
        r.IsValid.ShouldBeTrue();
    }
}
