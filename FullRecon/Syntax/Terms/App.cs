using Common;

namespace FullRecon.Syntax.Terms;

public class App(IInfo info, ITerm left, ITerm right) : ITerm
{
    public IInfo Info { get; } = info;
    public ITerm Left { get; } = left;
    public ITerm Right { get; } = right;
}
