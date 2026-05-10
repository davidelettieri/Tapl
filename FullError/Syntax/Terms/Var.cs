using Common;

namespace FullError.Syntax.Terms;

public sealed class Var : ITerm
{
    public IInfo Info { get; }
    public int Index { get; }
    public int ContextLength { get; }

    public Var(IInfo info, int index, int ctxl)
    {
        Info = info;
        Index = index;
        ContextLength = ctxl;
    }

    public override string ToString() => $"TmVar({Index},{ContextLength})";
}
