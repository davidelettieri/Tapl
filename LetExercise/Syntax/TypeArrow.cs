using Common;

namespace LetExercise.Syntax;

public class TypeArrow(IType from, IType to) : IType
{
    public IType From { get; } = from;
    public IType To { get; } = to;
}