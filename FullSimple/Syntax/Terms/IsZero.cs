using Common;

namespace FullSimple.Syntax.Terms;

public class IsZero : ITerm
{
    public IInfo Info { get; }
    public ITerm Term { get; }

    public IsZero(IInfo info, ITerm term)
    {
            Info = info;
            Term = term;
        }
}