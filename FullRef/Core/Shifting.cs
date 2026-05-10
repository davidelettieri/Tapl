using System;
using System.Linq;
using Common;
using FullRef.Syntax;
using FullRef.Syntax.Bindings;
using FullRef.Syntax.Terms;

namespace FullRef.Core;

public static class Shifting
{
    public static IType TypeMap(Func<int, TypeVar, IType> onVar, int c, IType t)
    {
        IType Walk(int c, IType t)
        {
            return t switch
            {
                TypeVar v => onVar(c, v),
                TypeBot b => b,
                TypeId i => i,
                TypeString ts => ts,
                TypeUnit u => u,
                TypeRecord r => new TypeRecord(r.Variants.Select(p => (p.Item1, Walk(c, p.Item2)))),
                TypeFloat f => f,
                TypeBool b => b,
                TypeNat n => n,
                TypeArrow a => new TypeArrow(Walk(c, a.From), Walk(c, a.To)),
                TypeRef r => new TypeRef(Walk(c, r.Type)),
                TypeSource s => new TypeSource(Walk(c, s.Type)),
                TypeSink s => new TypeSink(Walk(c, s.Type)),
                TypeTop ttop => ttop,
                TypeVariant tv => new TypeVariant(tv.Variants.Select(p => (p.Item1, Walk(c, p.Item2)))),
                _ => throw new InvalidOperationException()
            };
        }

        return Walk(c, t);
    }

    public static ITerm TmMap(Func<int, Var, ITerm> onVar,
        Func<int, IType, IType> onType,
        int c,
        ITerm t)
    {
        ITerm Walk(int c, ITerm t)
        {
            return t switch
            {
                Var var => onVar(c, var),
                Abs abs => new Abs(abs.Info, abs.V, onType(c, abs.Type), Walk(c + 1, abs.Body)),
                App app => new App(app.Info, Walk(c, app.Left), Walk(c, app.Right)),
                Assign assign => new Assign(assign.Info, Walk(c, assign.Left), Walk(c, assign.Right)),
                Deref deref => new Deref(deref.Info, Walk(c, deref.Term)),
                Let let => new Let(let.Info, let.Variable, Walk(c, let.LetTerm), Walk(c + 1, let.InTerm)),
                Inert i => new Inert(i.Info, onType(c, i.Type)),
                Fix fix => new Fix(fix.Info, Walk(c, fix.Term)),
                True e => e,
                False f => f,
                If ift => new If(ift.Info, Walk(c, ift.Condition), Walk(c, ift.Then), Walk(c, ift.Else)),
                Loc loc => loc,
                Ref reference => new Ref(reference.Info, Walk(c, reference.Term)),
                StringTerm s => s,
                Unit u => u,
                Proj p => new Proj(p.Info, Walk(c, p.Term), p.Label),
                Record r => new Record(r.Info, r.Fields.Select(p => (p.Item1, Walk(c, p.Item2))).ToList()),
                Ascribe a => new Ascribe(a.Info, Walk(c, a.Term), onType(c, a.Type)),
                Float f => f,
                TimesFloat tf => new TimesFloat(tf.Info, Walk(c, tf.Left), Walk(c, tf.Right)),
                Zero z => z,
                Succ s => new Succ(s.Info, Walk(c, s.Of)),
                Pred p => new Pred(p.Info, Walk(c, p.Of)),
                IsZero iz => new IsZero(iz.Info, Walk(c, iz.Term)),
                Tag tag => new Tag(tag.Info, tag.Label, Walk(c, tag.Term), onType(c, tag.Type)),
                Case @case => new Case(@case.Info, Walk(c, @case.Term),
                    @case.Cases.Select(p => (p.label, p.variable, Walk(c + 1, p.term)))),
                _ => throw new InvalidOperationException()
            };
        }

        return Walk(c, t);
    }

    private static IType TypeShiftAbove(int d, int c, IType t)
    {
        IType f(int c, TypeVar tv)
            => tv.X >= c ? new TypeVar(tv.X + d, tv.N + d) : new TypeVar(tv.X, tv.N + d);

        return TypeMap(f, c, t);
    }

    private static ITerm TermShiftAbove(int d, int c, ITerm t)
    {
        ITerm f(int c, Var v) =>
            v.Index >= c
                ? new Var(v.Info, v.Index + d, v.ContextLength + d)
                : new Var(v.Info, v.Index, v.ContextLength + d);

        IType OnType(int c, IType t) => TypeShiftAbove(d, c, t);

        return TmMap(f, OnType, c, t);
    }

    public static ITerm TermShift(int d, ITerm t) => TermShiftAbove(d, 0, t);

    public static IType TypeShift(int d, IType t) => TypeShiftAbove(d, 0, t);

    public static IBinding BindingShift(int d, IBinding bind) => bind switch
    {
        NameBinding nb => nb,
        TypeVarBind tvb => tvb,
        TermAbbBind tab => tab.Type is null
            ? new TermAbbBind(TermShift(d, tab.Term), null)
            : new TermAbbBind(TermShift(d, tab.Term), TypeShift(d, tab.Type)),
        VarBind vb => new VarBind(TypeShift(d, vb.Type)),
        TypeAbbBind tyab => new TypeAbbBind(TypeShift(d, tyab.Type)),
        _ => throw new InvalidOperationException()
    };
}