using Common;

namespace FullSimple.Syntax.Terms
{
    public class Ascribe : ITerm
    {
        public IInfo Info { get; }
        public ITerm Term { get; }
        public IType Type { get; }
        public Ascribe(IInfo info, ITerm term, IType type)
        {
            Info = info;
            Term = term;
            Type = type;
        }
    }
}
