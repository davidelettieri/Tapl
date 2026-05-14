using Common;

namespace FullUntyped.Syntax.Terms;

public sealed class False(IInfo info) : ITerm
{
    public IInfo Info { get; } = info;
}