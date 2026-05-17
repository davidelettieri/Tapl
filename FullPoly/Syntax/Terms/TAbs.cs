using Common;

namespace FullPoly.Syntax.Terms;

public sealed class TAbs(IInfo info, string typeVar, ITerm body) : ITerm
{
    public IInfo Info { get; } = info;
    public string TypeVar { get; } = typeVar;
    public ITerm Body { get; } = body;
}
