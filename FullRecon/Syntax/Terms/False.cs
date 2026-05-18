using Common;

namespace FullRecon.Syntax.Terms;

public class False(IInfo info) : ITerm
{
    public IInfo Info { get; } = info;
}
