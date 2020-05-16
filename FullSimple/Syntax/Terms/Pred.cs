using Common;

namespace FullSimple.Syntax.Terms
{
    public class Pred : ITerm
    {
        public IInfo Info { get; }
        public ITerm Of { get; }

        public Pred(IInfo info, ITerm of)
        {
            Info = info;
            Of = of;
        }
    }
}
