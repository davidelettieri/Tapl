using Common;

namespace FullRecon.Syntax.Terms;

public class Zero(IInfo info) : ITerm
{
    public IInfo Info { get; } = info;
}
