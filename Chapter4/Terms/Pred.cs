namespace Chapter4.Terms
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
