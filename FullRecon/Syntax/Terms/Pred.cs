using Common;

namespace FullRecon.Syntax.Terms;

public class Pred(IInfo info, ITerm of) : ITerm
{
    public IInfo Info { get; } = info;
    public ITerm Of { get; } = of;
}
