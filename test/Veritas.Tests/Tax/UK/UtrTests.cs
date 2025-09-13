using System;
using Veritas.Tax.UK;
using Xunit;
using Shouldly;

public class UtrTests
{
    [Theory]
    [InlineData("1123456789", true)]
    [InlineData("7660487647", true)]
    [InlineData("2123456789", false)]
    [InlineData("1123456780", false)]
    [InlineData("123456789", false)]
    [InlineData("11234A6789", false)]
    public void Validate(string input, bool expected)
    {
        Utr.TryValidate(input, out var r).ShouldBe(expected);
        r.IsValid.ShouldBe(expected);
    }

    [Fact]
    public void GenerateProducesValid()
    {
        Span<char> buffer = stackalloc char[10];
        Utr.TryGenerate(buffer, out var written).ShouldBeTrue();
        Utr.TryValidate(buffer[..written], out var r).ShouldBeTrue();
        r.IsValid.ShouldBeTrue();
    }
}

