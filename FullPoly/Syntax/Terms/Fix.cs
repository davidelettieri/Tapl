using Common;

namespace FullPoly.Syntax.Terms;

public sealed class Fix(IInfo info, ITerm term) : ITerm
{
    public IInfo Info { get; } = info;
    public ITerm Term { get; } = term;
}