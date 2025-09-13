using Veritas.Energy.DE;
using Xunit;
using Shouldly;

public class ZpnTests
{
    [Fact]
    public void GenerateAndValidate()
    {
        Span<char> buf = stackalloc char[40];
        Zpn.TryGenerate(buf, out var written).ShouldBeTrue();
        var s = new string(buf[..written]);
        Zpn.TryValidate(s, out var result).ShouldBeTrue();
        result.IsValid.ShouldBeTrue();
    }

    [Fact]
    public void InvalidPrefix()
    {
        Zpn.TryValidate("XX123", out var result).ShouldBeFalse();
        result.IsValid.ShouldBeFalse();
    }
}
