using System;
using Veritas.Identity;
using Xunit;
using Shouldly;

public class KsuidTests
{
    [Theory]
    [InlineData("0ujtsYcgvSTl8PAuAdqWYSMnLOv", true)]
    [InlineData("0ujtsYcgvSTl8PAuAdqWYSMnLO", false)]
    public void Validate(string input, bool expected)
    {
        Ksuid.TryValidate(input, out var r).ShouldBe(expected);
        r.IsValid.ShouldBe(expected);
    }

    [Fact]
    public void Generate_ProducesValid()
    {
        Span<char> buf = stackalloc char[27];
        Ksuid.TryGenerate(default, buf, out var w).ShouldBeTrue();
        w.ShouldBe(27);
        Ksuid.TryValidate(new string(buf), out var r).ShouldBeTrue();
        r.IsValid.ShouldBeTrue();
    }
}
