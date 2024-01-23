using Common;

namespace LetExercise.Syntax;

public class If : ITerm
{
    public IInfo Info { get; }
    public ITerm Condition { get; }
    public ITerm Then { get; }
    public ITerm Else { get; }

    public If(IInfo info, ITerm condition, ITerm then, ITerm @else)
    {
            Info = info;
            Condition = condition;
            Then = then;
            Else = @else;
        }
}