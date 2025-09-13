using System;
using Veritas.Identity;
using Xunit;
using Shouldly;

public class UlidTests
{
    [Theory]
    [InlineData("01ARZ3NDEKTSV4RRFFQ69G5FAV", true)]
    [InlineData("01ARZ3NDEKTSV4RRFFQ69G5FIV", false)]
    public void Validate(string input, bool expected)
    {
        Ulid.TryValidate(input, out var r).ShouldBe(expected);
        r.IsValid.ShouldBe(expected);
    }

    [Fact]
    public void GenerateProducesValid()
    {
        Span<char> buffer = stackalloc char[26];
        Ulid.TryGenerate(buffer, out var written).ShouldBeTrue();
        Ulid.TryValidate(buffer[..written], out var r).ShouldBeTrue();
        r.IsValid.ShouldBeTrue();
    }
}
