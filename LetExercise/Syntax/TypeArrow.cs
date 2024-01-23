using Common;

namespace LetExercise.Syntax;

public class TypeArrow : IType
{
    public IType From { get; }
    public IType To { get; }

    public TypeArrow(IType from, IType to)
    {
            From = from;
            To = to;
        }
}