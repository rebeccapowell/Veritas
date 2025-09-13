using System;
using Veritas.Telecom;
using Xunit;
using Shouldly;

public class ImsiTests
{
    [Theory]
    [InlineData("310260123456789", true)]
    [InlineData("31026A123456789", false)]
    [InlineData("12345678901234", false)]
    public void Validate(string input, bool expected)
    {
        Imsi.TryValidate(input, out var r).ShouldBe(expected);
        r.IsValid.ShouldBe(expected);
    }

    [Fact]
    public void GenerateProducesValid()
    {
        Span<char> buffer = stackalloc char[15];
        Imsi.TryGenerate(buffer, out var written).ShouldBeTrue();
        Imsi.TryValidate(buffer[..written], out var r).ShouldBeTrue();
        r.IsValid.ShouldBeTrue();
    }
}

