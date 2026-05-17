using Common;

namespace FullPoly.Syntax.Terms;

public sealed class False(IInfo info) : ITerm
{
    public IInfo Info { get; } = info;
}