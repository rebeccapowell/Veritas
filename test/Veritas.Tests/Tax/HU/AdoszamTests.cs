using Shouldly;
using Veritas.Tax.HU;
using Xunit;

namespace Veritas.Tests.Tax.HU;

public class AdoszamTests
{
    [Theory]
    [InlineData("12345676-1-31", true)]
    [InlineData("12345675-1-31", false)]
    [InlineData("12345A76-1-31", false)]
    [InlineData("12345676131", true)]
    public void Validate_Works(string input, bool expected)
    {
        Adoszam.TryValidate(input, out var result).ShouldBe(expected);
        if (expected)
            result.Value!.Value.ShouldBe("12345676131");
    }

    [Fact]
    public void Generate_RoundTrips()
    {
        Span<char> buffer = stackalloc char[11];
        Adoszam.TryGenerate(buffer, out var written).ShouldBeTrue();
        Adoszam.TryValidate(buffer[..written], out var result).ShouldBeTrue();
    }
}
