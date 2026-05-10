using Common;

namespace FullError.Syntax;

public sealed record TypeBot : IType;
public sealed record TypeTop : IType;
public sealed record TypeBool : IType;
public sealed record TypeArrow(IType From, IType To) : IType;
public sealed record TypeVar(int X, int N) : IType;
