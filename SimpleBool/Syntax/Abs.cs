using Common;

namespace SimpleBool.Syntax;

/// <summary>
/// Lambda abstraction term λx.y
/// </summary>
/// <param name="Body">The body of the lambda abstraction</param>
/// <param name="BoundedVariable">The bounded variable</param>
public record Abs(IInfo Info, ITerm Body, string BoundedVariable, IType Type) : ITerm;