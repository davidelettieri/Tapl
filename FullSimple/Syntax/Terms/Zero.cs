using Common;

namespace FullSimple.Syntax.Terms
{
    public class Zero : ITerm
    {
        public IInfo Info { get; }

        public Zero(IInfo info)
        {
            Info = info;
        }
    }
}
