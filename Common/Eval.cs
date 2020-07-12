using Common;

namespace Common
{
    public class Eval : ICommand
    {
        public ITerm Term { get; }
        public IInfo Info { get; }

        public Eval(IInfo info, ITerm term)
        {
            Info = info;
            Term = term;
        }
    }
}
