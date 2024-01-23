using Common;

namespace Untyped.Terms;

/// <summary>
/// Lambda abstraction term λx.y
/// </summary>
/// <param name="Body">The body of the lambda abstraction</param>
/// <param name="BoundedVariable">The bounded variable</param>
public record Abs(ITerm Body, string BoundedVariable) : ITerm;
