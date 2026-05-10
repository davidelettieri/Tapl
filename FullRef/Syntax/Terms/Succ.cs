using Common;

namespace FullRef.Syntax.Terms;

public sealed class Succ(IInfo info, ITerm of) : ITerm
{
    public IInfo Info { get; } = info;
    public ITerm Of { get; } = of;
}