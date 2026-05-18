using Common;

namespace FullPoly.Syntax.Terms;

public sealed class Pred(IInfo info, ITerm of) : ITerm
{
    public IInfo Info { get; } = info;
    public ITerm Of { get; } = of;
}