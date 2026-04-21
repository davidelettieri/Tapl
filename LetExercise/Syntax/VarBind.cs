using Common;

namespace LetExercise.Syntax;

public class VarBind(IType type) : IBinding
{
    public IType Type { get; } = type;
}