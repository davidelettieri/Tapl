using Common;

namespace FullRecon.Syntax.Terms;

public class True(IInfo info) : ITerm
{
    public IInfo Info { get; } = info;
}
