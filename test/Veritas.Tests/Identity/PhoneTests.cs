using Veritas.Identity;
using Xunit;
using Shouldly;

public class PhoneTests
{
    [Theory]
    [InlineData("+14155552671", true)]
    [InlineData("123-abc", false)]
    public void Validate(string input, bool expected)
    {
        Phone.TryValidate(input, out var r);
        r.IsValid.ShouldBe(expected);
    }
}
