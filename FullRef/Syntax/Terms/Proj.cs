using Common;

namespace FullRef.Syntax.Terms;

public sealed class Proj(IInfo info, ITerm term, string label) : ITerm
{
    public IInfo Info { get; } = info;
    public ITerm Term { get; } = term;
    public string Label { get; } = label;

    public override string ToString() => $"TmProj({Term},{Label})";
}