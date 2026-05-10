using System;
using Common;
using FullError.Syntax;
using FullError.Syntax.Bindings;
using FullError.Syntax.Terms;
using static FullError.Core.Shifting;

namespace FullError.Core;

public static class Typing
{
    private static bool IsTyAbb(Context ctx, int i) => ctx.GetBinding(i) is TypeAbbBind;

    private static IType GetTyAbb(Context ctx, int i)
        => ctx.GetBinding(i) switch
        {
            TypeAbbBind ta => ta.Type,
            _ => throw new NoRulesAppliesException()
        };

    private static IType ComputeType(Context ctx, IType t)
        => t switch
        {
            TypeVar tv when IsTyAbb(ctx, tv.X) => GetTyAbb(ctx, tv.X),
            _ => throw new NoRulesAppliesException()
        };

    private static IType SimplifyType(Context ctx, IType t)
    {
        try
        {
            var t1 = ComputeType(ctx, t);
            return SimplifyType(ctx, t1);
        }
        catch (NoRulesAppliesException)
        {
            return t;
        }
    }

    private static bool TypeEqual(Context ctx, IType s, IType t)
    {
        var ss = SimplifyType(ctx, s);
        var ts = SimplifyType(ctx, t);

        return (ss, ts) switch
        {
            (TypeBool, TypeBool) => true,
            (TypeBot, TypeBot) => true,
            (TypeTop, TypeTop) => true,
            (TypeArrow ta1, TypeArrow ta2) => TypeEqual(ctx, ta1.From, ta2.From) && TypeEqual(ctx, ta1.To, ta2.To),
            (TypeVar ti, _) when IsTyAbb(ctx, ti.X) => TypeEqual(ctx, GetTyAbb(ctx, ti.X), ts),
            (_, TypeVar ti) when IsTyAbb(ctx, ti.X) => TypeEqual(ctx, ss, GetTyAbb(ctx, ti.X)),
            (TypeVar ti, TypeVar tj) => ti.X == tj.X,
            _ => false
        };
    }

    public static bool Subtype(Context ctx, IType s, IType t)
    {
        if (TypeEqual(ctx, s, t))
            return true;

        var ss = SimplifyType(ctx, s);
        var ts = SimplifyType(ctx, t);

        return (ss, ts) switch
        {
            (TypeBot, _) => true,
            (_, TypeTop) => true,
            (TypeArrow ta1, TypeArrow ta2) => Subtype(ctx, ta2.From, ta1.From) && Subtype(ctx, ta1.To, ta2.To),
            _ => false
        };
    }

    public static IType Join(Context ctx, IType s, IType t)
    {
        if (Subtype(ctx, s, t)) return t;
        if (Subtype(ctx, t, s)) return s;

        var ss = SimplifyType(ctx, s);
        var ts = SimplifyType(ctx, t);

        return (ss, ts) switch
        {
            (TypeArrow ta1, TypeArrow ta2) => new TypeArrow(Meet(ctx, ta1.From, ta2.From), Join(ctx, ta1.To, ta2.To)),
            _ => new TypeTop()
        };
    }

    public static IType Meet(Context ctx, IType s, IType t)
    {
        if (Subtype(ctx, s, t)) return s;
        if (Subtype(ctx, t, s)) return t;

        var ss = SimplifyType(ctx, s);
        var ts = SimplifyType(ctx, t);

        return (ss, ts) switch
        {
            (TypeArrow ta1, TypeArrow ta2) => new TypeArrow(Join(ctx, ta1.From, ta2.From), Meet(ctx, ta1.To, ta2.To)),
            _ => new TypeBot()
        };
    }

    public static IType TypeOf(Context ctx, ITerm t)
    {
        return t switch
        {
            Var var => ctx.GetTypeFromContext(var.Index),
            Abs abs =>
                new TypeArrow(
                    abs.Type,
                    TypeShift(-1, TypeOf(ctx.AddBinding(abs.V, new VarBind(abs.Type)), abs.Body))),
            App app =>
                TypeOfApp(ctx, app),
            True => new TypeBool(),
            False => new TypeBool(),
            If ift =>
                TypeOfIf(ctx, ift),
            Error => new TypeBot(),
            Try tr => Join(ctx, TypeOf(ctx, tr.Term), TypeOf(ctx, tr.Handler)),
            _ => throw new InvalidOperationException($"Unexpected term: {t}")
        };
    }

    private static IType TypeOfApp(Context ctx, App app)
    {
        var tyT1 = TypeOf(ctx, app.Left);
        var tyT2 = TypeOf(ctx, app.Right);

        return SimplifyType(ctx, tyT1) switch
        {
            TypeArrow ta when Subtype(ctx, tyT2, ta.From) => ta.To,
            TypeArrow => throw new TaplTypingException(app.Info, "parameter type mismatch"),
            TypeBot => new TypeBot(),
            _ => throw new TaplTypingException(app.Info, "arrow type expected")
        };
    }

    private static IType TypeOfIf(Context ctx, If ift)
    {
        if (!Subtype(ctx, TypeOf(ctx, ift.Condition), new TypeBool()))
            throw new TaplTypingException(ift.Info, "guard of conditional not a boolean");

        return Join(ctx, TypeOf(ctx, ift.Then), TypeOf(ctx, ift.Else));
    }
}
