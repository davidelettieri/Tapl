using Common;

namespace FullError.Syntax.Terms;

public sealed class Abs(IInfo info, string v, IType type, ITerm body) : ITerm
{
    public IInfo Info { get; } = info;
    public string V { get; } = v;
    public IType Type { get; } = type;
    public ITerm Body { get; } = body;

    public override string ToString() => $"TmAbs({V},{Type},{Body})";
}
