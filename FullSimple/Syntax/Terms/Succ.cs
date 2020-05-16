using Common;

namespace FullSimple.Syntax.Terms
{
    public class Succ : ITerm
    {
        public IInfo Info { get; }
        public ITerm Of { get; }

        public Succ(IInfo info, ITerm of)
        {
            Info = info;
            Of = of;
        }
    }
}
