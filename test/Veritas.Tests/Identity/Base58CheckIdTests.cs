using Veritas.Identity;
using Xunit;
using Shouldly;

public class Base58CheckIdentityTests
{
    [Fact]
    public void Validate_Known()
    {
        var s = "1BoatSLRHtKNngkdXEeobR76b53LETtpyT";
        Base58Check.TryValidate(s, out var result).ShouldBeTrue();
        result.IsValid.ShouldBeTrue();
    }

    [Fact]
    public void Validate_Invalid()
    {
        Base58Check.TryValidate("0OIl", out var result).ShouldBeFalse();
        result.IsValid.ShouldBeFalse();
    }
}
