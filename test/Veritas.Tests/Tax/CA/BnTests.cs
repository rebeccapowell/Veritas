using Veritas.Tax.CA;
using Xunit;
using Shouldly;

public class BnTests
{
    [Fact]
    public void Validate_Works()
    {
        var valid = "660487646";
        Bn.TryValidate(valid, out var result);
        result.IsValid.ShouldBeTrue();
        Bn.TryValidate(valid[..8] + '0', out var invalid);
        invalid.IsValid.ShouldBeFalse();
    }

    [Fact]
    public void GenerateProducesValid()
    {
        Span<char> buffer = stackalloc char[9];
        Bn.TryGenerate(buffer, out var written).ShouldBeTrue();
        Bn.TryValidate(buffer[..written], out var result);
        result.IsValid.ShouldBeTrue();
    }
}
