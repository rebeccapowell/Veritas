using Veritas.Tax.CN;
using Xunit;
using Shouldly;

public class UsccTests
{
    [Fact]
    public void Validate_Works()
    {
        const string valid = "UCQWD18YGFCXRT9YFY";
        Uscc.TryValidate(valid, out var result);
        result.IsValid.ShouldBeTrue();
        Uscc.TryValidate(valid[..17] + '0', out var invalid);
        invalid.IsValid.ShouldBeFalse();
    }

    [Fact]
    public void GenerateProducesValid()
    {
        Span<char> buffer = stackalloc char[18];
        Uscc.TryGenerate(buffer, out var written).ShouldBeTrue();
        Uscc.TryValidate(buffer[..written], out var result);
        result.IsValid.ShouldBeTrue();
    }
}
