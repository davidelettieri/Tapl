using Common;

namespace FullError.Syntax.Terms;

public sealed class Error(IInfo info) : ITerm
{
    public IInfo Info { get; } = info;
}
