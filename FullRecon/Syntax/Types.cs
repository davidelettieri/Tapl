using Common;

namespace FullRecon.Syntax;

public sealed record TypeArrow(IType From, IType To) : IType;
public sealed record TypeBool : IType;
public sealed record TypeNat : IType;
public sealed record TypeId(string Name) : IType;
