using Common;

namespace LetExercise.Syntax;

/// <summary>
/// Lambda abstraction term λx.y
/// </summary>
/// <param name="body">The body of the lambda abstraction</param>
/// <param name="bv">The bounded variable</param>
public class Abs(IInfo info, ITerm body, string bv, IType type) : ITerm
{
    public IInfo Info { get; } = info;
    public ITerm Body { get; } = body;
    public string BoundedVariable { get; } = bv;
    public IType Type { get; } = type;
}