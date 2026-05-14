using Common;

namespace FullUntyped.Syntax.Terms;

public class Unit(IInfo info) : ITerm
{
    public IInfo Info { get; } = info;
}