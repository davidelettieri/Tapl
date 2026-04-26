using Common;

namespace FullSimple.Syntax.Terms;

public class StringTerm(string value) : ITerm
{
    public string Value { get; } = value;

    public override string ToString() => $"TmString({Value})";
}