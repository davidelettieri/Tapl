using Common;

namespace FullRecon.Syntax.Terms;

public class IsZero(IInfo info, ITerm term) : ITerm
{
    public IInfo Info { get; } = info;
    public ITerm Term { get; } = term;
}
