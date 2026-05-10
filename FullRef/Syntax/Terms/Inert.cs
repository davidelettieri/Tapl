using Common;

namespace FullRef.Syntax.Terms;

public sealed class Inert(IInfo info, IType type) : ITerm
{
    public IInfo Info { get; } = info;
    public IType Type { get; } = type;
}