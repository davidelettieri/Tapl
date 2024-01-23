using Common;

namespace LetExercise.Syntax;

public class True : ITerm
{
    public IInfo Info { get; }
    public True(IInfo info)
    {
            Info = info;
        }
}