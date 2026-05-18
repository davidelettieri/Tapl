using Common;

namespace FullPoly.Syntax.Terms;

public sealed class Unpack(IInfo info, string typeVar, string v, ITerm package, ITerm body) : ITerm
{
    public IInfo Info { get; } = info;
    public string TypeVar { get; } = typeVar;
    public string V { get; } = v;
    public ITerm Package { get; } = package;
    public ITerm Body { get; } = body;
}
