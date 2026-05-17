using System;
using Common;
using FullUpdate.Syntax;

namespace FullUpdate.Visitors;

public sealed class OKindVisitor : FullUpdateBaseVisitor<Func<Context, IKind>>
{
    private static readonly KindVisitor KindVisitor = new();

    public override Func<Context, IKind> VisitOkind_kn(FullUpdateParser.Okind_knContext context)
        => KindVisitor.Visit(context.kind());

    public override Func<Context, IKind> VisitOkind_empty(FullUpdateParser.Okind_emptyContext context)
        => _ => new KnStar();
}
