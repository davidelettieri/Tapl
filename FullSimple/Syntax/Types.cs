using Common;
using System.Collections.Generic;

namespace FullSimple.Syntax;

public record TypeArrow(IType From, IType To) : IType;
public record TypeBool : IType;
public record TypeFloat : IType;
public record TypeId(string Name) : IType;
public record TypeNat : IType;
public record TypeRecord(IEnumerable<(string, IType)> Variants) : IType;
public record TypeString : IType;
public record TypeUnit : IType;
public record TypeVar(int X, int N) : IType;
public record TypeVariant(IEnumerable<(string, IType)> Variants) : IType;