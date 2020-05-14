using Common;

namespace Arith.Terms
{
    public class Pred : ITerm
    {
        public ITerm Of { get; }

        public Pred(ITerm of)
        {
            Of = of;
        }
    }


}
