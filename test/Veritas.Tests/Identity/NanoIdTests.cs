using System;
using Veritas.Identity;
using Xunit;
using Shouldly;

public class NanoIdTests
{
    [Theory]
    [InlineData("0123456789ABCDEFGHIJK", true)]
    [InlineData("0123456789ABCDEFGH#K", false)]
    public void Validate(string input, bool expected)
    {
        NanoId.TryValidate(input, out var r);
        r.IsValid.ShouldBe(expected);
    }

    [Fact]
    public void GenerateProducesValid()
    {
        Span<char> buffer = stackalloc char[21];
        NanoId.TryGenerate(buffer, out var written).ShouldBeTrue();
        NanoId.TryValidate(buffer[..written], out var r);
        r.IsValid.ShouldBeTrue();
    }
}
