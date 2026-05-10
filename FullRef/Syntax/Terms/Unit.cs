using Common;

namespace FullRef.Syntax.Terms;

public class Unit(IInfo info) : ITerm
{
    public IInfo Info { get; } = info;
}