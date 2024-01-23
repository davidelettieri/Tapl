using Common;

namespace FullSimple.Syntax.Terms;

public class False : ITerm
{
    public IInfo Info { get; }
    public False(IInfo info)
    {
            Info = info;
        }
}