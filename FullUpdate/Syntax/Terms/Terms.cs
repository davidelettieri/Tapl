using System.Collections.Generic;
using Common;
using FullUpdate.Syntax;

namespace FullUpdate.Syntax.Terms;

public sealed class Var(IInfo info, int index, int contextLength) : ITerm
{
    public IInfo Info { get; } = info;
    public int Index { get; } = index;
    public int ContextLength { get; } = contextLength;
}

public sealed class Abs(IInfo info, string v, IType type, ITerm body) : ITerm
{
    public IInfo Info { get; } = info;
    public string V { get; } = v;
    public IType Type { get; } = type;
    public ITerm Body { get; } = body;
}

public sealed class App(IInfo info, ITerm left, ITerm right) : ITerm
{
    public IInfo Info { get; } = info;
    public ITerm Left { get; } = left;
    public ITerm Right { get; } = right;
}

// Type abstraction: lambda X<:T. t
public sealed class TAbs(IInfo info, string typeVar, IType bound, ITerm body) : ITerm
{
    public IInfo Info { get; } = info;
    public string TypeVar { get; } = typeVar;
    public IType Bound { get; } = bound;
    public ITerm Body { get; } = body;
}

// Type application: t [T]
public sealed class TApp(IInfo info, ITerm term, IType typeArg) : ITerm
{
    public IInfo Info { get; } = info;
    public ITerm Term { get; } = term;
    public IType TypeArg { get; } = typeArg;
}

public sealed class StringTerm(IInfo info, string value) : ITerm
{
    public IInfo Info { get; } = info;
    public string Value { get; } = value;
}

// Pack: {*T1, t2} as T3
public sealed class Pack(IInfo info, IType witnessType, ITerm term, IType existType) : ITerm
{
    public IInfo Info { get; } = info;
    public IType WitnessType { get; } = witnessType;
    public ITerm Term { get; } = term;
    public IType ExistType { get; } = existType;
}

// Unpack: let {X,x} = t1 in t2
public sealed class Unpack(IInfo info, string typeVar, string v, ITerm package, ITerm body) : ITerm
{
    public IInfo Info { get; } = info;
    public string TypeVar { get; } = typeVar;
    public string V { get; } = v;
    public ITerm Package { get; } = package;
    public ITerm Body { get; } = body;
}

public sealed class Record(IInfo info, List<(string Label, Variance Var, ITerm Term)> fields) : ITerm
{
    public IInfo Info { get; } = info;
    public List<(string Label, Variance Var, ITerm Term)> Fields { get; } = fields;
}

public sealed class Proj(IInfo info, ITerm term, string label) : ITerm
{
    public IInfo Info { get; } = info;
    public ITerm Term { get; } = term;
    public string Label { get; } = label;
}

public sealed class True(IInfo info) : ITerm { public IInfo Info { get; } = info; }
public sealed class False(IInfo info) : ITerm { public IInfo Info { get; } = info; }

public sealed class If(IInfo info, ITerm condition, ITerm then, ITerm els) : ITerm
{
    public IInfo Info { get; } = info;
    public ITerm Condition { get; } = condition;
    public ITerm Then { get; } = then;
    public ITerm Else { get; } = els;
}

public sealed class Zero(IInfo info) : ITerm { public IInfo Info { get; } = info; }

public sealed class Succ(IInfo info, ITerm of) : ITerm
{
    public IInfo Info { get; } = info;
    public ITerm Of { get; } = of;
}

public sealed class Pred(IInfo info, ITerm of) : ITerm
{
    public IInfo Info { get; } = info;
    public ITerm Of { get; } = of;
}

public sealed class IsZero(IInfo info, ITerm term) : ITerm
{
    public IInfo Info { get; } = info;
    public ITerm Term { get; } = term;
}

public sealed class Unit(IInfo info) : ITerm { public IInfo Info { get; } = info; }

// Record update: t1 <- l = t2
public sealed class Update(IInfo info, ITerm record, string label, ITerm newValue) : ITerm
{
    public IInfo Info { get; } = info;
    public ITerm Record { get; } = record;
    public string Label { get; } = label;
    public ITerm NewValue { get; } = newValue;
}

public sealed class Float(IInfo info, double value) : ITerm
{
    public IInfo Info { get; } = info;
    public double Value { get; } = value;
}

public sealed class TimesFloat(IInfo info, ITerm left, ITerm right) : ITerm
{
    public IInfo Info { get; } = info;
    public ITerm Left { get; } = left;
    public ITerm Right { get; } = right;
}

public sealed class Let(IInfo info, string variable, ITerm letTerm, ITerm inTerm) : ITerm
{
    public IInfo Info { get; } = info;
    public string Variable { get; } = variable;
    public ITerm LetTerm { get; } = letTerm;
    public ITerm InTerm { get; } = inTerm;
}

public sealed class Inert(IInfo info, IType type) : ITerm
{
    public IInfo Info { get; } = info;
    public IType Type { get; } = type;
}

public sealed class Fix(IInfo info, ITerm term) : ITerm
{
    public IInfo Info { get; } = info;
    public ITerm Term { get; } = term;
}

public sealed class Ascribe(IInfo info, ITerm term, IType type) : ITerm
{
    public IInfo Info { get; } = info;
    public ITerm Term { get; } = term;
    public IType Type { get; } = type;
}
