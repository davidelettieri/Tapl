using System;
using Common;
using FullUpdate.Syntax;

namespace FullUpdate.Visitors;

/// <summary>
/// Handles both kind and akind grammar rules in a single visitor to avoid constructor cycles.
/// </summary>
public sealed class KindVisitor : FullUpdateBaseVisitor<Func<Context, IKind>>
{
    public override Func<Context, IKind> VisitKind_darrow(FullUpdateParser.Kind_darrowContext context)
    {
        var left = Visit(context.akind());
        var right = Visit(context.kind());
        return ctx => new KnArr(left(ctx), right(ctx));
    }

    public override Func<Context, IKind> VisitKind_akind(FullUpdateParser.Kind_akindContext context)
        => Visit(context.akind());

    public override Func<Context, IKind> VisitAkind_star(FullUpdateParser.Akind_starContext context)
        => _ => new KnStar();

    public override Func<Context, IKind> VisitAkind_paren(FullUpdateParser.Akind_parenContext context)
        => Visit(context.kind());
}
