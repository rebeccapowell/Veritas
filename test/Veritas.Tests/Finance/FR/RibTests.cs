using Veritas.Finance.FR;

namespace Veritas.Tests.Finance.FR;

public class RibTests
{
    [Theory]
    [InlineData("20041 01005 0500013M026 06", true)]
    [InlineData("20041010050500013M02606", true)]
    [InlineData("20041010050500013M02607", false)]
    public void Validate_Works(string input, bool expected)
    {
        var ok = Rib.TryValidate(input, out var result);
        Assert.Equal(expected, ok);
        Assert.Equal(expected, result.IsValid);
    }

    [Fact]
    public void Generate_ProducesValid()
    {
        Span<char> dest = stackalloc char[23];
        Assert.True(Rib.TryGenerate(dest, out var written));
        Assert.True(Rib.TryValidate(new string(dest[..written]), out _));
    }
}

