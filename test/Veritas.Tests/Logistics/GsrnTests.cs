using Shouldly;
using Veritas.Logistics;
using Xunit;

namespace Veritas.Tests.Logistics;

public class GsrnTests
{
    [Theory]
    [InlineData("123456789012345675", true)]
    [InlineData("123456789012345674", false)]
    [InlineData("12345678901234567", false)]
    [InlineData("12345678901234567A", false)]
    public void Validate_Works(string input, bool expected)
        => Gsrn.TryValidate(input, out var r).ShouldBe(expected);

    [Fact]
    public void Generate_RoundTrips()
    {
        Span<char> buffer = stackalloc char[18];
        Gsrn.TryGenerate(buffer, out var written).ShouldBeTrue();
        Gsrn.TryValidate(buffer[..written], out var r).ShouldBeTrue();
    }
}
