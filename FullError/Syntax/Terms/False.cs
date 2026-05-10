using Common;

namespace FullError.Syntax.Terms;

public sealed class False(IInfo info) : ITerm
{
    public IInfo Info { get; } = info;
}
