using Common;

namespace FullSimple.Syntax.Terms
{
    public class Unit : ITerm
    {
        public IInfo Info { get; }
        public Unit(IInfo info)
        {
            Info = info;
        }
    }
}
