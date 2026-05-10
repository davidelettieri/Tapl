using Common;

namespace FullRef.Syntax.Terms;

public sealed class Loc(IInfo info, int location) : ITerm
{
    public IInfo Info { get; } = info;
    public int Location { get; } = location;

    public override string ToString() => $"TmLoc({Location})";
}
