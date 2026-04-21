using Common;

namespace LetExercise.Syntax;

public class False(IInfo info) : ITerm
{
    public IInfo Info { get; } = info;
}