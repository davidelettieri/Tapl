namespace Chapter4.Terms
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
