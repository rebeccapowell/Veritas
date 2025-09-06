using System;
using System.Net.Mail;
using Veritas;

namespace Veritas.Identity;

/// <summary>Represents a validated email address.</summary>
public readonly struct EmailValue
{
    public string Value { get; }
    public EmailValue(string value) => Value = value;
}

/// <summary>Provides validation for email addresses.</summary>
public static class Email
{
    public static bool TryValidate(ReadOnlySpan<char> input, out ValidationResult<EmailValue> result)
    {
        var str = input.ToString();
        try
        {
            var addr = new MailAddress(str);
            result = new ValidationResult<EmailValue>(true, new EmailValue(addr.Address), ValidationError.None);
        }
        catch
        {
            result = new ValidationResult<EmailValue>(false, default, ValidationError.Format);
        }
        return true;
    }
}

