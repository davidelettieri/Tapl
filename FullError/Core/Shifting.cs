using System;
using Common;
using FullError.Syntax;
using FullError.Syntax.Bindings;
using FullError.Syntax.Terms;

namespace FullError.Core;

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
                TypeTop tp => tp,
                TypeBool b => b,
                TypeArrow a => new TypeArrow(Walk(c, a.From), Walk(c, a.To)),
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
                True tr => tr,
                False f => f,
                If ift => new If(ift.Info, Walk(c, ift.Condition), Walk(c, ift.Then), Walk(c, ift.Else)),
                Error err => err,
                Try tr => new Try(tr.Info, Walk(c, tr.Term), Walk(c, tr.Handler)),
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
