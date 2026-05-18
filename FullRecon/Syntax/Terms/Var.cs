using Common;

namespace FullRecon.Syntax.Terms;

public class Var(IInfo info, int index, int contextLength) : ITerm
{
    public IInfo Info { get; } = info;
    public int Index { get; } = index;
    public int ContextLength { get; } = contextLength;
}
