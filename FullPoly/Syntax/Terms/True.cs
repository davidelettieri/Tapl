using Common;

namespace FullPoly.Syntax.Terms;

public sealed class True(IInfo info) : ITerm
{
    public IInfo Info { get; } = info;
}