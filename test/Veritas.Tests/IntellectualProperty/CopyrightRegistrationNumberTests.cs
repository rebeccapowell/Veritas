using Veritas.IntellectualProperty;
using Veritas;
using Shouldly;
using Xunit;

public class CopyrightRegistrationNumberTests
{
    [Fact]
    public void Generate_Validates()
    {
        Span<char> dst = stackalloc char[12];
        CopyrightRegistrationNumber.TryGenerate(new GenerationOptions { Seed = 6 }, dst, out var w).ShouldBeTrue();
        var s = new string(dst[..w]);
        CopyrightRegistrationNumber.TryValidate(s, out var r).ShouldBeTrue();
        r.IsValid.ShouldBeTrue();
    }

    [Fact]
    public void Invalid_Format()
    {
        CopyrightRegistrationNumber.TryValidate("TX2023ABCD12", out var r).ShouldBeFalse();
        r.IsValid.ShouldBeFalse();
    }
}
