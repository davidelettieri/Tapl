using System;
using Common;
using Untyped.Terms;

namespace Untyped.Syntax;

public static class Printing
{
    public static string PrintTerm(Context ctx, ITerm t)
    {
        switch (t)
        {
            case Abs abs:
                var (newContext, xp) = ctx.PickFreshName(abs.BoundedVariable);
                return $"(lambda {xp}.{PrintTerm(newContext, abs.Body)})";
            case App app:
                return $"({PrintTerm(ctx, app.Left)}{PrintTerm(ctx, app.Right)})";
            case Var var:
                if (ctx.Length == var.ContextLength)
                    return ctx.IndexToName(var.Index);
                return "[bad index]";
            default:
                throw new InvalidOperationException();
        }
    }
}