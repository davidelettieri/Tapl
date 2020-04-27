using Common;

namespace Chapter7
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
