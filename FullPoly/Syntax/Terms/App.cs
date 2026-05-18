using Common;

namespace FullPoly.Syntax.Terms;

/// <summary>
/// Application term (xy)
/// </summary>
/// <param name="left">The first term in the application</param>
/// <param name="right">The second term in the application</param>
public sealed class App(IInfo info, ITerm left, ITerm right) : ITerm
{
    public IInfo Info { get; } = info;
    public ITerm Left { get; } = left;
    public ITerm Right { get; } = right;

    public override string ToString() => $"TmApp({Left},{Right})";
}