using Common;

namespace FullPoly.Syntax.Terms;

public sealed class Zero(IInfo info) : ITerm
{
    public IInfo Info { get; } = info;
}