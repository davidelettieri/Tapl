using Common;

namespace FullError.Syntax.Terms;

public sealed class Try(IInfo info, ITerm term, ITerm handler) : ITerm
{
    public IInfo Info { get; } = info;
    public ITerm Term { get; } = term;
    public ITerm Handler { get; } = handler;
}
