using System;
using Common;
using FullUpdate.Core;
using FullUpdate.Syntax;

namespace FullUpdate.Visitors;

public sealed class OTypeVisitor : FullUpdateBaseVisitor<Func<Context, IType>>
{
    private static readonly TypeVisitor TypeVisitor = new();
    private static readonly KindVisitor KindVisitor = new();

    public override Func<Context, IType> VisitOtype_leq(FullUpdateParser.Otype_leqContext context)
        => TypeVisitor.Visit(context.type());

    public override Func<Context, IType> VisitOtype_kn(FullUpdateParser.Otype_knContext context)
    {
        var kind = KindVisitor.Visit(context.kind());
        return ctx => Typing.MakeTop(kind(ctx));
    }

    public override Func<Context, IType> VisitOtype_empty(FullUpdateParser.Otype_emptyContext context)
        => _ => new TypeTop();
}
