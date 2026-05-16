using System.Collections.Generic;
using Common;

namespace FullUpdate.Syntax;

// Kinds
public interface IKind { }
public sealed record KnStar : IKind;
public sealed record KnArr(IKind From, IKind To) : IKind;

// Variance
public enum Variance { Covariant, Invariant }

// Types
public sealed record TypeVar(int X, int N) : IType;
public sealed record TypeAbs(string Name, IKind Kind, IType Body) : IType;
public sealed record TypeApp(IType T1, IType T2) : IType;
public sealed record TypeAll(string Name, IType Bound, IType Body) : IType;
public sealed record TypeSome(string Name, IType Bound, IType Body) : IType;
public sealed record TypeRecord(IEnumerable<(string Label, Variance Var, IType Type)> Fields) : IType;
public sealed record TypeTop : IType;
public sealed record TypeArrow(IType From, IType To) : IType;
public sealed record TypeBool : IType;
public sealed record TypeNat : IType;
public sealed record TypeUnit : IType;
public sealed record TypeString : IType;
public sealed record TypeFloat : IType;
public sealed record TypeId(string Name) : IType;
