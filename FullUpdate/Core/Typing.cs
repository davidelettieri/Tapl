using System;
using System.Collections.Generic;
using System.Linq;
using Common;
using FullUpdate.Syntax;
using FullUpdate.Syntax.Bindings;
using FullUpdate.Syntax.Terms;
using static FullUpdate.Core.Shifting;
using static FullUpdate.Core.Substitution;

namespace FullUpdate.Core;

public static class Typing
{
    // ---- maketop ----
    public static IType MakeTop(IKind k) => k switch
    {
        KnStar => new TypeTop(),
        KnArr knArr => new TypeAbs("_", knArr.From, MakeTop(knArr.To)),
        _ => throw new InvalidOperationException()
    };

    // ---- TypeAbb helpers ----
    private static bool IsTyAbb(Context ctx, int i) => ctx.GetBinding(i) is TypeAbbBind;

    private static IType GetTyAbb(Context ctx, int i) =>
        ctx.GetBinding(i) switch
        {
            TypeAbbBind tab => tab.Type,
            _ => throw new NoRulesAppliesException()
        };

    // ---- Compute / Simplify ----
    private static IType ComputeType(Context ctx, IType t) => t switch
    {
        TypeApp { T1: TypeAbs abs } app => TypeSubstTop(app.T2, abs.Body),
        TypeVar tv when IsTyAbb(ctx, tv.X) => GetTyAbb(ctx, tv.X),
        _ => throw new NoRulesAppliesException()
    };

    public static IType SimplifyType(Context ctx, IType t)
    {
        // For TyApp, first simplify T1
        var t2 = t is TypeApp app ? new TypeApp(SimplifyType(ctx, app.T1), app.T2) : t;
        try
        {
            return SimplifyType(ctx, ComputeType(ctx, t2));
        }
        catch (NoRulesAppliesException)
        {
            return t2;
        }
    }

    // ---- Type equality ----
    public static bool TyEqv(Context ctx, IType s, IType t)
    {
        var ss = SimplifyType(ctx, s);
        var ts = SimplifyType(ctx, t);
        return (ss, ts) switch
        {
            (TypeArrow { From: var s1, To: var s2 }, TypeArrow { From: var t1, To: var t2 })
                => TyEqv(ctx, s1, t1) && TyEqv(ctx, s2, t2),
            (TypeAbs abs1, TypeAbs abs2)
                => abs1.Kind == abs2.Kind
                   || (abs1.Kind is KnStar && abs2.Kind is KnStar)
                   || KindEquals(abs1.Kind, abs2.Kind)
                   ? TyEqv(ctx.AddName(abs1.Name), abs1.Body, abs2.Body) : false,
            (TypeApp a1, TypeApp a2)
                => TyEqv(ctx, a1.T1, a2.T1) && TyEqv(ctx, a1.T2, a2.T2),
            (TypeBool, TypeBool) => true,
            (TypeNat, TypeNat) => true,
            (TypeRecord r1, TypeRecord r2) => TyEqvRecord(ctx, r1, r2),
            (TypeSome some1, TypeSome some2) => TyEqv(ctx, some1.Bound, some2.Bound)
                && TyEqv(ctx.AddName(some1.Name), some1.Body, some2.Body),
            (TypeUnit, TypeUnit) => true,
            (TypeId b1, TypeId b2) => b1.Name == b2.Name,
            (TypeVar tv, _) when IsTyAbb(ctx, tv.X) => TyEqv(ctx, GetTyAbb(ctx, tv.X), ts),
            (_, TypeVar tv) when IsTyAbb(ctx, tv.X) => TyEqv(ctx, ss, GetTyAbb(ctx, tv.X)),
            (TypeVar v1, TypeVar v2) => v1.X == v2.X,
            (TypeAll all1, TypeAll all2) => TyEqv(ctx, all1.Bound, all2.Bound)
                && TyEqv(ctx.AddName(all1.Name), all1.Body, all2.Body),
            (TypeString, TypeString) => true,
            (TypeFloat, TypeFloat) => true,
            (TypeTop, TypeTop) => true,
            _ => false
        };
    }

    private static bool TyEqvRecord(Context ctx, TypeRecord r1, TypeRecord r2)
    {
        var f1 = r1.Fields.ToList();
        var f2 = r2.Fields.ToList();
        if (f1.Count != f2.Count) return false;
        foreach (var (l2, v2, ty2) in f2)
        {
            var m = f1.FirstOrDefault(x => x.Label == l2);
            if (m.Label is null) return false;
            if (m.Var != v2) return false;
            if (!TyEqv(ctx, m.Type, ty2)) return false;
        }
        return true;
    }

    // ---- Kinding ----
    private static IKind GetKind(IInfo fi, Context ctx, int i)
        => ctx.GetBinding(i) switch
        {
            TypeVarBind tvb => KindOf(ctx, tvb.Bound),
            TypeAbbBind { Kind: KnStar k } tab => k,
            TypeAbbBind { Kind: KnArr k } tab => k,
            TypeAbbBind { Kind: null } => throw new TaplTypingException(fi, $"No kind recorded for variable {ctx.IndexToName(i)}"),
            _ => throw new TaplTypingException(fi, $"getkind: Wrong kind of binding for variable {ctx.IndexToName(i)}")
        };

    public static IKind KindOf(Context ctx, IType t)
    {
        var fi = new UnknownInfo();
        return t switch
        {
            TypeArrow arr =>
                KindOf(ctx, arr.From) is KnStar
                    ? KindOf(ctx, arr.To) is KnStar
                        ? KnStarSingleton
                        : throw new TaplTypingException(fi, "star kind expected")
                    : throw new TaplTypingException(fi, "star kind expected"),
            TypeVar tv => GetKind(fi, ctx, tv.X),
            TypeAbs abs =>
                KnArr(abs.Kind, KindOf(ctx.AddBinding(abs.Name, new TypeVarBind(MakeTop(abs.Kind))), abs.Body)),
            TypeApp app =>
                KindOf(ctx, app.T1) switch
                {
                    KnArr kArr when KindEquals(KindOf(ctx, app.T2), kArr.From) => kArr.To,
                    KnArr _ => throw new TaplTypingException(fi, "parameter kind mismatch"),
                    _ => throw new TaplTypingException(fi, "arrow kind expected")
                },
            TypeAll all =>
                KindOf(ctx.AddBinding(all.Name, new TypeVarBind(all.Bound)), all.Body) is KnStar
                    ? KnStarSingleton
                    : throw new TaplTypingException(fi, "Kind * expected"),
            TypeRecord rec =>
                rec.Fields.All(f => KindOf(ctx, f.Type) is KnStar)
                    ? KnStarSingleton
                    : throw new TaplTypingException(fi, "Kind * expected"),
            TypeSome some =>
                KindOf(ctx.AddBinding(some.Name, new TypeVarBind(some.Bound)), some.Body) is KnStar
                    ? KnStarSingleton
                    : throw new TaplTypingException(fi, "Kind * expected"),
            _ => KnStarSingleton
        };
    }

    private static readonly KnStar KnStarSingleton = new KnStar();

    private static bool KindEquals(IKind a, IKind b) => (a, b) switch
    {
        (KnStar, KnStar) => true,
        (KnArr kArr1, KnArr kArr2) => KindEquals(kArr1.From, kArr2.From) && KindEquals(kArr1.To, kArr2.To),
        _ => false
    };

    private static KnArr KnArr(IKind from, IKind to) => new KnArr(from, to);

    public static void CheckKindStar(IInfo fi, Context ctx, IType t)
    {
        if (!KindEquals(KindOf(ctx, t), KnStarSingleton))
            throw new TaplTypingException(fi, "Kind * expected");
    }

    // ---- Promote ----
    private static IType Promote(Context ctx, IType t) => t switch
    {
        TypeVar tv => ctx.GetBinding(tv.X) switch
        {
            TypeVarBind tvb => tvb.Bound,
            _ => throw new NoRulesAppliesException()
        },
        TypeApp app => new TypeApp(Promote(ctx, app.T1), app.T2),
        _ => throw new NoRulesAppliesException()
    };

    // ---- Lcst ----
    public static IType Lcst(Context ctx, IType t)
    {
        var s = SimplifyType(ctx, t);
        try { return Lcst(ctx, Promote(ctx, s)); }
        catch (NoRulesAppliesException) { return s; }
    }

    // ---- Subtype ----
    public static bool Subtype(Context ctx, IType s, IType t)
    {
        if (TyEqv(ctx, s, t)) return true;
        var ss = SimplifyType(ctx, s);
        var ts = SimplifyType(ctx, t);
        return (ss, ts) switch
        {
            (TypeVar, _) => Subtype(ctx, Promote(ctx, ss), ts),
            (_, TypeTop) => true,
            (TypeArrow { From: var s1, To: var s2 }, TypeArrow { From: var t1, To: var t2 })
                => Subtype(ctx, t1, s1) && Subtype(ctx, s2, t2),
            (TypeAll all1, TypeAll all2) =>
                Subtype(ctx, all1.Bound, all2.Bound) && Subtype(ctx, all2.Bound, all1.Bound) &&
                Subtype(ctx.AddBinding(all1.Name, new TypeVarBind(all2.Bound)), all1.Body, all2.Body),
            (TypeRecord r1, TypeRecord r2) =>
                r2.Fields.All(ft2 =>
                {
                    var m = r1.Fields.FirstOrDefault(x => x.Label == ft2.Label);
                    if (m.Label is null) return false;
                    return (m.Var, ft2.Var) switch
                    {
                        // invariant field in supertype: must be exactly the same type (both directions)
                        (Variance.Invariant, Variance.Invariant) =>
                            Subtype(ctx, m.Type, ft2.Type) && Subtype(ctx, ft2.Type, m.Type),
                        // covariant field: T1 <: T2 (invariant in source satisfies covariant requirement)
                        (Variance.Invariant, Variance.Covariant) => Subtype(ctx, m.Type, ft2.Type),
                        (Variance.Covariant, Variance.Covariant) => Subtype(ctx, m.Type, ft2.Type),
                        _ => false
                    };
                }),
            (TypeSome some1, TypeSome some2) =>
                Subtype(ctx, some1.Bound, some2.Bound) && Subtype(ctx, some2.Bound, some1.Bound) &&
                Subtype(ctx.AddBinding(some1.Name, new TypeVarBind(some2.Bound)), some1.Body, some2.Body),
            (TypeAbs abs1, TypeAbs abs2) =>
                KindEquals(abs1.Kind, abs2.Kind) &&
                Subtype(ctx.AddBinding(abs1.Name, new TypeVarBind(MakeTop(abs1.Kind))), abs1.Body, abs2.Body),
            (TypeApp, _) => Subtype(ctx, Promote(ctx, ss), ts),
            _ => false
        };
    }

    // ---- Join ----
    public static IType Join(Context ctx, IType s, IType t)
    {
        if (Subtype(ctx, s, t)) return t;
        if (Subtype(ctx, t, s)) return s;
        var ss = SimplifyType(ctx, s);
        var ts = SimplifyType(ctx, t);
        return (ss, ts) switch
        {
            (TypeRecord r1, TypeRecord r2) => JoinRecord(ctx, r1, r2),
            (TypeAll all1, TypeAll all2) =>
                !(Subtype(ctx, all1.Bound, all2.Bound) && Subtype(ctx, all2.Bound, all1.Bound))
                    ? new TypeTop()
                    : new TypeAll(all1.Name, all1.Bound,
                        Join(ctx.AddBinding(all1.Name, new TypeVarBind(all2.Bound)), all2.Bound, all2.Body)),
            (TypeArrow arr1, TypeArrow arr2) =>
                JoinArrow(ctx, arr1, arr2),
            _ => new TypeTop()
        };
    }

    private static IType JoinRecord(Context ctx, TypeRecord r1, TypeRecord r2)
    {
        var f1 = r1.Fields.ToList();
        var f2 = r2.Fields.ToList();
        var labelsS = f1.Select(f => f.Label).ToList();
        var labelsT = f2.Select(f => f.Label).ToList();
        var commonLabels = labelsS.Where(l => labelsT.Contains(l)).ToList();
        var common = commonLabels.Select(l =>
        {
            var fS = f1.First(x => x.Label == l);
            var fT = f2.First(x => x.Label == l);
            var vi = fS.Var == fT.Var ? fS.Var : Variance.Invariant;
            return (l, vi, Join(ctx, fS.Type, fT.Type));
        }).ToList();
        return new TypeRecord(common);
    }

    private static IType JoinArrow(Context ctx, TypeArrow arr1, TypeArrow arr2)
    {
        try
        {
            return new TypeArrow(Meet(ctx, arr1.From, arr2.From), Join(ctx, arr1.To, arr2.To));
        }
        catch (NotSupportedException)
        {
            return new TypeTop();
        }
    }

    // ---- Meet ----
    public static IType Meet(Context ctx, IType s, IType t)
    {
        if (Subtype(ctx, s, t)) return s;
        if (Subtype(ctx, t, s)) return t;
        var ss = SimplifyType(ctx, s);
        var ts = SimplifyType(ctx, t);
        return (ss, ts) switch
        {
            (TypeRecord r1, TypeRecord r2) => MeetRecord(ctx, r1, r2),
            (TypeAll all1, TypeAll all2) =>
                !(Subtype(ctx, all1.Bound, all2.Bound) && Subtype(ctx, all2.Bound, all1.Bound))
                    ? throw new NotSupportedException("meet")
                    : new TypeAll(all1.Name, all1.Bound,
                        Meet(ctx.AddBinding(all1.Name, new TypeVarBind(all2.Bound)), all2.Bound, all2.Body)),
            (TypeArrow arr1, TypeArrow arr2) =>
                new TypeArrow(Join(ctx, arr1.From, arr2.From), Meet(ctx, arr1.To, arr2.To)),
            _ => throw new NotSupportedException("meet")
        };
    }

    private static IType MeetRecord(Context ctx, TypeRecord r1, TypeRecord r2)
    {
        var f1 = r1.Fields.ToList();
        var f2 = r2.Fields.ToList();
        var labelsS = f1.Select(f => f.Label).ToList();
        var labelsT = f2.Select(f => f.Label).ToList();
        var allLabels = labelsS.Concat(labelsT.Where(l => !labelsS.Contains(l))).ToList();
        var all = allLabels.Select(l =>
        {
            bool inS = labelsS.Contains(l);
            bool inT = labelsT.Contains(l);
            if (inS && inT)
            {
                var fS = f1.First(x => x.Label == l);
                var fT = f2.First(x => x.Label == l);
                var vi = fS.Var == fT.Var ? fS.Var : Variance.Covariant;
                return (l, vi, Meet(ctx, fS.Type, fT.Type));
            }
            else if (inS)
            {
                var fS = f1.First(x => x.Label == l);
                return (l, fS.Var, fS.Type);
            }
            else
            {
                var fT = f2.First(x => x.Label == l);
                return (l, fT.Var, fT.Type);
            }
        }).ToList();
        return new TypeRecord(all);
    }

    // ---- TypeOf ----
    public static IType TypeOf(Context ctx, ITerm t)
    {
        switch (t)
        {
            case Var var: return ctx.GetTypeFromContext(var.Index);

            case Abs abs:
                CheckKindStar(abs.Info, ctx, abs.Type);
                var ctx1 = ctx.AddBinding(abs.V, new VarBind(abs.Type));
                var tyBody = TypeOf(ctx1, abs.Body);
                return new TypeArrow(abs.Type, TypeShift(-1, tyBody));

            case App app:
                var tyApp1 = TypeOf(ctx, app.Left);
                var tyApp2 = TypeOf(ctx, app.Right);
                var lcstApp = Lcst(ctx, tyApp1);
                if (lcstApp is TypeArrow ta)
                {
                    if (Subtype(ctx, tyApp2, ta.From)) return ta.To;
                    throw new TaplTypingException(app.Info, "parameter type mismatch");
                }
                throw new TaplTypingException(app.Info, "arrow type expected");

            case Ascribe asc:
                CheckKindStar(asc.Info, ctx, asc.Type);
                if (Subtype(ctx, TypeOf(ctx, asc.Term), asc.Type)) return asc.Type;
                throw new TaplTypingException(asc.Info, "body of as-term does not have the expected type");

            case TAbs tabs:
                var ctxTAbs = ctx.AddBinding(tabs.TypeVar, new TypeVarBind(tabs.Bound));
                var tyT2 = TypeOf(ctxTAbs, tabs.Body);
                return new TypeAll(tabs.TypeVar, tabs.Bound, tyT2);

            case TApp tapp:
                var tyTApp1 = TypeOf(ctx, tapp.Term);
                var lcstTApp = Lcst(ctx, tyTApp1);
                if (lcstTApp is TypeAll all1)
                {
                    if (!Subtype(ctx, tapp.TypeArg, all1.Bound))
                        throw new TaplTypingException(tapp.Info, "type parameter type mismatch");
                    return TypeSubstTop(tapp.TypeArg, all1.Body);
                }
                throw new TaplTypingException(tapp.Info, "universal type expected");

            case StringTerm: return new TypeString();

            case Pack pack:
                CheckKindStar(pack.Info, ctx, pack.ExistType);
                var simpPack = SimplifyType(ctx, pack.ExistType);
                if (simpPack is TypeSome some)
                {
                    if (!Subtype(ctx, pack.WitnessType, some.Bound))
                        throw new TaplTypingException(pack.Info, "hidden type not a subtype of bound");
                    var tyU = TypeOf(ctx, pack.Term);
                    var tyU2 = TypeSubstTop(pack.WitnessType, some.Body);
                    if (Subtype(ctx, tyU, tyU2)) return pack.ExistType;
                    throw new TaplTypingException(pack.Info, "doesn't match declared type");
                }
                throw new TaplTypingException(pack.Info, "existential type expected");

            case Unpack unpack:
                var tyUnpack1 = TypeOf(ctx, unpack.Package);
                var lcstUnpack = Lcst(ctx, tyUnpack1);
                if (lcstUnpack is TypeSome some2)
                {
                    var ctx2 = ctx.AddBinding(unpack.TypeVar, new TypeVarBind(some2.Bound));
                    var ctx3 = ctx2.AddBinding(unpack.V, new VarBind(some2.Body));
                    var tyUnpack2 = TypeOf(ctx3, unpack.Body);
                    var tyResult = TypeShift(-2, tyUnpack2);
                    // Check for scope extrusion: if X or x indices appear free in the result type, the existential type escapes
                    if (HasNegativeVar(tyResult))
                        throw new TaplTypingException(unpack.Info, "Scope extrusion: existential type variable escapes");
                    return tyResult;
                }
                throw new TaplTypingException(unpack.Info, "existential type expected");

            case Record rec:
                var fieldTys = rec.Fields.Select(f => (f.Label, f.Var, TypeOf(ctx, f.Term))).ToList();
                return new TypeRecord(fieldTys);

            case Proj proj:
                var lcstProj = Lcst(ctx, TypeOf(ctx, proj.Term));
                if (lcstProj is TypeRecord trec)
                {
                    var m = trec.Fields.FirstOrDefault(f => f.Label == proj.Label);
                    if (m.Label is null)
                        throw new TaplTypingException(proj.Info, $"label {proj.Label} not found");
                    return m.Type;
                }
                throw new TaplTypingException(proj.Info, "Expected record type");

            case True: return new TypeBool();
            case False: return new TypeBool();

            case If ift:
                if (Subtype(ctx, TypeOf(ctx, ift.Condition), new TypeBool()))
                    return Join(ctx, TypeOf(ctx, ift.Then), TypeOf(ctx, ift.Else));
                throw new TaplTypingException(ift.Info, "guard of conditional not a boolean");

            case Zero: return new TypeNat();

            case Succ succ:
                if (Subtype(ctx, TypeOf(ctx, succ.Of), new TypeNat())) return new TypeNat();
                throw new TaplTypingException(succ.Info, "argument of succ is not a number");

            case Pred pred:
                if (Subtype(ctx, TypeOf(ctx, pred.Of), new TypeNat())) return new TypeNat();
                throw new TaplTypingException(pred.Info, "argument of pred is not a number");

            case IsZero iz:
                if (Subtype(ctx, TypeOf(ctx, iz.Term), new TypeNat())) return new TypeBool();
                throw new TaplTypingException(iz.Info, "argument of iszero is not a number");

            case Unit: return new TypeUnit();

            case Update upd:
                var tyUpd1 = TypeOf(ctx, upd.Record);
                var tyUpd2 = TypeOf(ctx, upd.NewValue);
                var lcstUpd = Lcst(ctx, tyUpd1);
                if (lcstUpd is TypeRecord trecUpd)
                {
                    var m = trecUpd.Fields.FirstOrDefault(f => f.Label == upd.Label);
                    if (m.Label is null)
                        throw new TaplTypingException(upd.Info, $"label {upd.Label} not found");
                    if (m.Var != Variance.Invariant)
                        throw new TaplTypingException(upd.Info, "field not invariant");
                    if (Subtype(ctx, tyUpd2, m.Type)) return tyUpd1;
                    throw new TaplTypingException(upd.Info, "type of new field value doesn't match");
                }
                throw new TaplTypingException(upd.Info, "Expected record type");

            case Float: return new TypeFloat();

            case TimesFloat tf:
                if (Subtype(ctx, TypeOf(ctx, tf.Left), new TypeFloat())
                    && Subtype(ctx, TypeOf(ctx, tf.Right), new TypeFloat()))
                    return new TypeFloat();
                throw new TaplTypingException(tf.Info, "argument of timesfloat is not a number");

            case Let let:
                var tyLet1 = TypeOf(ctx, let.LetTerm);
                var ctxLet = ctx.AddBinding(let.Variable, new VarBind(tyLet1));
                return TypeShift(-1, TypeOf(ctxLet, let.InTerm));

            case Inert i: return i.Type;

            case Fix fix:
                var tyFix = TypeOf(ctx, fix.Term);
                var lcstFix = Lcst(ctx, tyFix);
                if (lcstFix is TypeArrow taFix)
                {
                    if (Subtype(ctx, taFix.To, taFix.From)) return taFix.To;
                    throw new TaplTypingException(fix.Info, "result of body not compatible with domain");
                }
                throw new TaplTypingException(fix.Info, "arrow type expected");

            default:
                throw new InvalidOperationException($"TypeOf: unhandled {t.GetType().Name}");
        }
    }

    // ---- HasNegativeVar ----
    // Returns true if there's a TypeVar with a negative index (caused by TypeShift of an escaped variable)
    private static bool HasNegativeVar(IType t) => t switch
    {
        TypeVar tv => tv.X < 0,
        TypeArrow arr => HasNegativeVar(arr.From) || HasNegativeVar(arr.To),
        TypeAll all => HasNegativeVar(all.Bound) || HasNegativeVar(all.Body),
        TypeSome some => HasNegativeVar(some.Bound) || HasNegativeVar(some.Body),
        TypeAbs abs => HasNegativeVar(abs.Body),
        TypeApp app => HasNegativeVar(app.T1) || HasNegativeVar(app.T2),
        TypeRecord rec => rec.Fields.Any(f => HasNegativeVar(f.Type)),
        _ => false
    };

    // ---- CheckBinding ----
    public static IBinding CheckBinding(IInfo fi, Context ctx, IBinding bind)
    {
        switch (bind)
        {
            case NameBinding: return bind;
            case TypeVarBind tvb:
                KindOf(ctx, tvb.Bound);
                return bind;
            case TypeAbbBind { Kind: null } tab:
                return new TypeAbbBind(tab.Type, KindOf(ctx, tab.Type));
            case TypeAbbBind tab:
                if (KindEquals(KindOf(ctx, tab.Type), tab.Kind!)) return bind;
                throw new TaplTypingException(fi, "Kind of binding does not match declared kind");
            case VarBind: return bind;
            case TermAbbBind { Type: null } tab:
                return new TermAbbBind(tab.Term, TypeOf(ctx, tab.Term));
            case TermAbbBind tab:
                if (Subtype(ctx, TypeOf(ctx, tab.Term), tab.Type!)) return bind;
                throw new TaplTypingException(fi, "Type of binding does not match declared type");
            default:
                throw new InvalidOperationException();
        }
    }
}
