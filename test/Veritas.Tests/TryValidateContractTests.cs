using System;
using System.Collections.Generic;
using System.Reflection;
using Shouldly;
using Xunit;

public class TryValidateContractTests
{
    private delegate bool Validator<T>(ReadOnlySpan<char> input, out Veritas.ValidationResult<T> result);

    public static IEnumerable<object[]> Validators()
    {
        foreach (Type type in typeof(Veritas.GenerationOptions).Assembly.GetExportedTypes())
            foreach (MethodInfo method in type.GetMethods(BindingFlags.Public | BindingFlags.Static))
            {
                ParameterInfo[] args = method.GetParameters();
                if (method.Name != "TryValidate" || args.Length != 2 || args[0].ParameterType != typeof(ReadOnlySpan<char>)) continue;
                Type? result = args[1].ParameterType.GetElementType();
                if (result?.IsGenericType == true && result.GetGenericTypeDefinition() == typeof(Veritas.ValidationResult<>))
                    yield return new object[] { type.FullName!, method };
            }
    }

    [Theory]
    [MemberData(nameof(Validators))]
    public void MalformedInputs_DoNotThrow(string name, MethodInfo method)
    {
        Type type = method.GetParameters()[1].ParameterType.GetElementType()!.GetGenericArguments()[0];
        typeof(TryValidateContractTests).GetMethod(nameof(Check), BindingFlags.NonPublic | BindingFlags.Static)!
            .MakeGenericMethod(type).Invoke(null, new object[] { name, method });
    }

    private static void Check<T>(string name, MethodInfo method)
    {
        Validator<T> validate = method.CreateDelegate<Validator<T>>();
        for (int length = 1; length <= 40; length++)
        {
            Exception? error = Record.Exception(() => validate(new string('\u0661', length), out _));
            error.ShouldBeNull($"{name} threw on {length} Unicode digits");
        }
        foreach (string input in new[] { "", "\ud800", "\u0661", "\uff11", new string('\u0661', 16), new string('\uff11', 34), "DE12\u0661", "11Y12345678901\u0661T", new string('2', 15) + "+" })
        {
            Exception? error = Record.Exception(() => validate(input, out _));
            error.ShouldBeNull($"{name} threw on {input}");
        }
    }
}
