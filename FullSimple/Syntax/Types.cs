using Common;
using System.Collections.Generic;

namespace FullSimple.Syntax;

public sealed record TypeArrow(IType From, IType To) : IType;
public sealed record TypeBool : IType;
public sealed record TypeFloat : IType;
public sealed record TypeId(string Name) : IType;
public sealed record TypeNat : IType;
public sealed record TypeRecord(IEnumerable<(string, IType)> Variants) : IType;
public sealed record TypeString : IType;
public sealed record TypeUnit : IType;
public sealed record TypeVar(int X, int N) : IType;
public sealed record TypeVariant(IEnumerable<(string, IType)> Variants) : IType;