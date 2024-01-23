using Common;

namespace LetExercise.Syntax;

/// <summary>
/// Lambda abstraction term λx.y
/// </summary>
public class Abs : ITerm
{
    public IInfo Info { get; }
    public ITerm Body { get; }
    public string BoundedVariable { get; }
    public IType Type { get; }

    /// <summary>
    /// Lambda abstraction term λx.y
    /// </summary>
    /// <param name="body">The body of the lambda abstraction</param>
    /// <param name="bv">The bounded variable</param>
    public Abs(IInfo info, ITerm body, string bv, IType type)
    {
            Info = info;
            Body = body;
            BoundedVariable = bv;
            Type = type;
        }
}