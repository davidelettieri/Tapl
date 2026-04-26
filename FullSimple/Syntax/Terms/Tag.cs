using Common;

namespace FullSimple.Syntax.Terms;

public sealed class Tag(IInfo info, string label, ITerm term, IType type)
    : ITerm
{
    public IInfo Info { get; } = info;
    public string Label { get; } = label;
    public ITerm Term { get; } = term;
    public IType Type { get; } = type;

    public override string ToString() => $"TmTag({Label},{Term},{Type})";
}