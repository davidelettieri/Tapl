using System;
using Common;
using FullPoly.Syntax.Bindings;

namespace FullPoly.Visitors;

public sealed class TyBinderVisitor : FullPolyBaseVisitor<Func<Context, IBinding>>
{
    private static readonly TypeVisitor TypeVisitor = new();

    public override Func<Context, IBinding> VisitTybinder_type(FullPolyParser.Tybinder_typeContext context)
    {
        var type = TypeVisitor.Visit(context.type());
        return ctx => new TypeAbbBind(type(ctx));
    }

    public override Func<Context, IBinding> VisitTybinder_empty(FullPolyParser.Tybinder_emptyContext context)
        => _ => new TypeVarBind();
}
