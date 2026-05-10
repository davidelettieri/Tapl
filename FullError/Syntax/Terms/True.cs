using Common;

namespace FullError.Syntax.Terms;

public sealed class True(IInfo info) : ITerm
{
    public IInfo Info { get; } = info;
}
