using System.Collections.Generic;
using Common;

namespace FullRef.Syntax;

public sealed record TypeBot : IType;
public sealed record TypeArrow(IType From, IType To) : IType;
public sealed record TypeBool : IType;
public sealed record TypeFloat : IType;
public sealed record TypeId(string Name) : IType;
public sealed record TypeNat : IType;
public sealed record TypeRecord(IEnumerable<(string, IType)> Variants) : IType;
public sealed record TypeRef(IType Type) : IType;
public sealed record TypeSink(IType Type) : IType;
public sealed record TypeSource(IType Type) : IType;
public sealed record TypeString : IType;
public sealed record TypeTop : IType;
public sealed record TypeUnit : IType;
public sealed record TypeVar(int X, int N) : IType;
public sealed record TypeVariant(IEnumerable<(string, IType)> Variants) : IType;