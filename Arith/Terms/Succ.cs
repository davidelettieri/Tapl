using Common;

namespace Arith.Terms
{
    public class Succ : ITerm
    {
        public ITerm Of { get; }

        public Succ(ITerm of)
        {
            Of = of;
        }
    }


}
