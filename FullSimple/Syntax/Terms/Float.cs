using Common;

namespace FullSimple.Syntax.Terms;

public sealed class Float(IInfo info, float value) : ITerm
{
    public IInfo Info { get; } = info;
    public float Value { get; } = value;
}