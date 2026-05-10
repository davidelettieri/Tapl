using Common;

namespace FullError.Syntax.Terms;

public sealed class App(IInfo info, ITerm left, ITerm right) : ITerm
{
    public IInfo Info { get; } = info;
    public ITerm Left { get; } = left;
    public ITerm Right { get; } = right;

    public override string ToString() => $"TmApp({Left},{Right})";
}
