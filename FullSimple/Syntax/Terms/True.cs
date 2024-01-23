using Common;

namespace FullSimple.Syntax.Terms;

public class True : ITerm
{
    public IInfo Info { get; }
    public True(IInfo info)
    {
            Info = info;
        }
}