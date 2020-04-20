using Common;

namespace Chapter4.Terms
{
    public class If : ITerm
    {
        public ITerm Condition { get; }
        public ITerm Then { get; }
        public ITerm Else { get; }

        public If(ITerm condition, ITerm then, ITerm @else)
        {
            Condition = condition;
            Then = then;
            Else = @else;
        }
    }


}
