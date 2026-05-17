using Common;

namespace FullPoly.Syntax.Terms;

public sealed class TimesFloat(IInfo info, ITerm left, ITerm right) : ITerm
{
    public IInfo Info { get; } = info;
    public ITerm Left { get; } = left;
    public ITerm Right { get; } = right;
}