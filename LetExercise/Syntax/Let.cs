using Common;

namespace LetExercise.Syntax;

public class Let(IInfo info, string variable, ITerm letTerm, ITerm inTerm)
    : ITerm
{
    public IInfo Info { get; } = info;
    public string Variable { get; } = variable;
    public ITerm LetTerm { get; } = letTerm;
    public ITerm InTerm { get; } = inTerm;
}