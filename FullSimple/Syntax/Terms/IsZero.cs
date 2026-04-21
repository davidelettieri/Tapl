using Common;

namespace FullSimple.Syntax.Terms;

public sealed class IsZero(IInfo info, ITerm term) : ITerm
{
    public IInfo Info { get; } = info;
    public ITerm Term { get; } = term;
}