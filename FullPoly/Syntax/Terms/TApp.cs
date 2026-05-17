using Common;

namespace FullPoly.Syntax.Terms;

public sealed class TApp(IInfo info, ITerm term, IType typeArg) : ITerm
{
    public IInfo Info { get; } = info;
    public ITerm Term { get; } = term;
    public IType TypeArg { get; } = typeArg;
}
