using Veritas.Transportation;
using Xunit;
using Shouldly;

public class TrainUicNumberTests
{
    [Fact]
    public void GeneratedValueValidates()
    {
        Span<char> buffer = stackalloc char[12];
        TrainUicNumber.TryGenerate(buffer, out var written).ShouldBeTrue();
        TrainUicNumber.TryValidate(buffer[..written], out var result).ShouldBeTrue();
        result.IsValid.ShouldBeTrue();
        buffer[written - 1] = buffer[written - 1] == '0' ? '1' : '0';
        TrainUicNumber.TryValidate(buffer[..written], out var bad).ShouldBeFalse();
        bad.IsValid.ShouldBeFalse();
    }
}
