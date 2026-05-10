using Common;

namespace FullRef.Syntax.Terms;

public sealed class Deref(IInfo info, ITerm term) : ITerm
{
    public IInfo Info { get; } = info;
    public ITerm Term { get; } = term;

    public override string ToString() => $"TmDeref({Term})";
}
