using Veritas.Healthcare.Snomed;
using Veritas;
using Shouldly;
using Xunit;

public class SctIdTests
{
    [Fact]
    public void Generate_Validates()
    {
        Span<char> dst = stackalloc char[10];
        SctId.TryGenerate(10, new GenerationOptions { Seed = 42 }, dst, out var written).ShouldBeTrue();
        var s = new string(dst[..written]);
        SctId.TryValidate(s, out var r);
        r.IsValid.ShouldBeTrue();
    }

    [Fact]
    public void Invalid_Checksum()
    {
        Span<char> dst = stackalloc char[9];
        SctId.TryGenerate(9, new GenerationOptions { Seed = 1 }, dst, out var w).ShouldBeTrue();
        var s = new string(dst[..w]);
        // corrupt last digit
        char bad = s[^1] == '0' ? '1' : '0';
        var badStr = s[..^1] + bad;
        SctId.TryValidate(badStr, out var r);
        r.IsValid.ShouldBeFalse();
    }
}
