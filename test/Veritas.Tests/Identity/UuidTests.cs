using Veritas.Identity;
using Xunit;
using Shouldly;

public class UuidTests
{
    [Fact]
    public void Validate_Works()
    {
        var guid = System.Guid.NewGuid().ToString();
        Uuid.TryValidate(guid, out var result);
        result.IsValid.ShouldBeTrue();
    }
}

