using Common;

namespace FullRef.Syntax.Terms;

public sealed class Assign(IInfo info, ITerm left, ITerm right) : ITerm
{
    public IInfo Info { get; } = info;
    public ITerm Left { get; } = left;
    public ITerm Right { get; } = right;

    public override string ToString() => $"TmAssign({Left},{Right})";
}
