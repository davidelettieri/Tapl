using Common;

namespace FullUntyped.Syntax.Terms;

public sealed class Ascribe(IInfo info, ITerm term, IType type) : ITerm
{
    public IInfo Info { get; } = info;
    public ITerm Term { get; } = term;
    public IType Type { get; } = type;
}