using Common;

namespace FullUntyped.Syntax.Terms;

public sealed class Zero(IInfo info) : ITerm
{
    public IInfo Info { get; } = info;
}