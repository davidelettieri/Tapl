using Common;

namespace LetExercise.Syntax;

public class True(IInfo info) : ITerm
{
    public IInfo Info { get; } = info;
}