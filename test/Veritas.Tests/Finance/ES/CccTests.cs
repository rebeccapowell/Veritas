using Veritas.Finance.ES;

namespace Veritas.Tests.Finance.ES;

public class CccTests
{
    [Fact]
    public void Validate_Works()
    {
        Span<char> dest = stackalloc char[20];
        Assert.True(Ccc.TryGenerate(dest, out var written));
        var s = new string(dest[..written]);
        Assert.True(Ccc.TryValidate(s, out var result));
        dest[8] = dest[8] == '0' ? '1' : '0';
        Assert.False(Ccc.TryValidate(new string(dest[..written]), out _));
    }

    [Fact]
    public void Generate_ProducesValid()
    {
        Span<char> dest = stackalloc char[20];
        Assert.True(Ccc.TryGenerate(dest, out var written));
        Assert.True(Ccc.TryValidate(new string(dest[..written]), out _));
    }
}

