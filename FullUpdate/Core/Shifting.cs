using System;
using System.Collections.Generic;
using System.Linq;
using Common;
using FullUpdate.Syntax;
using FullUpdate.Syntax.Bindings;
using FullUpdate.Syntax.Terms;

namespace FullUpdate.Core;

public static class Shifting
{
    public static IType TypeMap(Func<int, TypeVar, IType> onVar, int c, IType t)
    {
        IType Walk(int c, IType t) => t switch
        {
            TypeVar v => onVar(c, v),
            TypeId i => i,
            TypeString ts => ts,
            TypeUnit u => u,
            TypeFloat f => f,
            TypeBool b => b,
            TypeNat n => n,
            TypeTop top => top,
            TypeArrow a => new TypeArrow(Walk(c, a.From), Walk(c, a.To)),
            TypeRecord r => new TypeRecord(r.Fields.Select(p => (p.Label, p.Var, Walk(c, p.Type)))),
            TypeAll all => new TypeAll(all.Name, Walk(c, all.Bound), Walk(c + 1, all.Body)),
            TypeSome some => new TypeSome(some.Name, Walk(c, some.Bound), Walk(c + 1, some.Body)),
            TypeAbs abs => new TypeAbs(abs.Name, abs.Kind, Walk(c + 1, abs.Body)),
            TypeApp app => new TypeApp(Walk(c, app.T1), Walk(c, app.T2)),
            _ => throw new InvalidOperationException($"TypeMap: unhandled {t.GetType().Name}")
        };
        return Walk(c, t);
    }

    public static ITerm TmMap(
        Func<int, Var, ITerm> onVar,
        Func<int, IType, IType> onType,
        int c,
        ITerm t)
    {
        ITerm Walk(int c, ITerm t) => t switch
        {
            Var var => onVar(c, var),
            Abs abs => new Abs(abs.Info, abs.V, onType(c, abs.Type), Walk(c + 1, abs.Body)),
            App app => new App(app.Info, Walk(c, app.Left), Walk(c, app.Right)),
            TAbs tabs => new TAbs(tabs.Info, tabs.TypeVar, onType(c, tabs.Bound), Walk(c + 1, tabs.Body)),
            TApp tapp => new TApp(tapp.Info, Walk(c, tapp.Term), onType(c, tapp.TypeArg)),
            StringTerm s => s,
            Pack pack => new Pack(pack.Info, onType(c, pack.WitnessType), Walk(c, pack.Term), onType(c, pack.ExistType)),
            Unpack unpack => new Unpack(unpack.Info, unpack.TypeVar, unpack.V, Walk(c, unpack.Package), Walk(c + 2, unpack.Body)),
            Proj proj => new Proj(proj.Info, Walk(c, proj.Term), proj.Label),
            Record rec => new Record(rec.Info, rec.Fields.Select(f => (f.Label, f.Var, Walk(c, f.Term))).ToList()),
            True e => e,
            False f => f,
            If ift => new If(ift.Info, Walk(c, ift.Condition), Walk(c, ift.Then), Walk(c, ift.Else)),
            Zero z => z,
            Succ s => new Succ(s.Info, Walk(c, s.Of)),
            Pred p => new Pred(p.Info, Walk(c, p.Of)),
            IsZero iz => new IsZero(iz.Info, Walk(c, iz.Term)),
            Update upd => new Update(upd.Info, Walk(c, upd.Record), upd.Label, Walk(c, upd.NewValue)),
            Let let => new Let(let.Info, let.Variable, Walk(c, let.LetTerm), Walk(c + 1, let.InTerm)),
            Unit u => u,
            Inert i => new Inert(i.Info, onType(c, i.Type)),
            Float f => f,
            TimesFloat tf => new TimesFloat(tf.Info, Walk(c, tf.Left), Walk(c, tf.Right)),
            Fix fix => new Fix(fix.Info, Walk(c, fix.Term)),
            Ascribe asc => new Ascribe(asc.Info, Walk(c, asc.Term), onType(c, asc.Type)),
            _ => throw new InvalidOperationException($"TmMap: unhandled {t.GetType().Name}")
        };
        return Walk(c, t);
    }

    internal static IType TypeShiftAbove(int d, int c, IType t)
        => TypeMap((c, tv) => tv.X >= c ? new TypeVar(tv.X + d, tv.N + d) : new TypeVar(tv.X, tv.N + d), c, t);

    private static ITerm TermShiftAbove(int d, int c, ITerm t)
        => TmMap(
            (c, v) => v.Index >= c
                ? new Var(v.Info, v.Index + d, v.ContextLength + d)
                : new Var(v.Info, v.Index, v.ContextLength + d),
            (c, ty) => TypeShiftAbove(d, c, ty),
            c, t);

    public static ITerm TermShift(int d, ITerm t) => TermShiftAbove(d, 0, t);
    public static IType TypeShift(int d, IType t) => TypeShiftAbove(d, 0, t);

    public static IBinding BindingShift(int d, IBinding bind) => bind switch
    {
        NameBinding nb => nb,
        VarBind vb => new VarBind(TypeShift(d, vb.Type)),
        TypeVarBind tvb => new TypeVarBind(TypeShift(d, tvb.Bound)),
        TypeAbbBind tab => new TypeAbbBind(TypeShift(d, tab.Type), tab.Kind),
        TermAbbBind tab => new TermAbbBind(
            TermShift(d, tab.Term),
            tab.Type is null ? null : TypeShift(d, tab.Type)),
        _ => throw new InvalidOperationException()
    };
}
