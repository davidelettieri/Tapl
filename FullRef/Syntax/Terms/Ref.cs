using Common;

namespace FullRef.Syntax.Terms;

public sealed class Ref(IInfo info, ITerm term) : ITerm
{
    public IInfo Info { get; } = info;
    public ITerm Term { get; } = term;

    public override string ToString() => $"TmRef({Term})";
}
