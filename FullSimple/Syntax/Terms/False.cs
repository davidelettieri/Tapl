using Common;

namespace FullSimple.Syntax.Terms;

public sealed class False(IInfo info) : ITerm
{
    public IInfo Info { get; } = info;
}