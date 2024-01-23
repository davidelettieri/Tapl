using Common;

namespace FullSimple.Syntax.Terms;

public class Inert : ITerm
{
    public IInfo Info { get; }
    public IType Type { get; }
    public Inert(IInfo info, IType type)
    {
            Info = info;
            Type = type;
        }
}