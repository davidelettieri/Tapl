using Common;

namespace Common
{
    public class Eval : ICommand
    {
        public ITerm Term { get; }

        public Eval(ITerm term)
        {
            Term = term;
        }
    }
}
