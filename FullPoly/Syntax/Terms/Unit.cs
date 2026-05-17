using Common;

namespace FullPoly.Syntax.Terms;

public class Unit(IInfo info) : ITerm
{
    public IInfo Info { get; } = info;
}