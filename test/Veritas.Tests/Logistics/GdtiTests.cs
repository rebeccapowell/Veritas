using Veritas.Logistics;

namespace Veritas.Tests.Logistics;

public class GdtiTests
{
    [Theory]
    [InlineData("12345678901231", true)]
    [InlineData("12345678901232", false)]
    public void Validate_Works(string input, bool expected)
    {
        var ok = Gdti.TryValidate(input, out var result);
        Assert.Equal(expected, ok);
        Assert.Equal(expected, result.IsValid);
    }

    [Fact]
    public void Generate_ProducesValid()
    {
        Span<char> dest = stackalloc char[14];
        Assert.True(Gdti.TryGenerate(dest, out var written));
        Assert.True(Gdti.TryValidate(new string(dest[..written]), out _));
    }
}

