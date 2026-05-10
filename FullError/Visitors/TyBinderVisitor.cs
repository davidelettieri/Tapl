using System;
using Common;
using FullError.Syntax.Bindings;

namespace FullError.Visitors;

public sealed class TyBinderVisitor : FullErrorBaseVisitor<Func<Context, IBinding>>
{
    private static readonly TypeVisitor TypeVisitor = new();

    public override Func<Context, IBinding> VisitTybinder_empty(FullErrorParser.Tybinder_emptyContext context)
        => _ => new TypeVarBind();

    public override Func<Context, IBinding> VisitTybinder_type(FullErrorParser.Tybinder_typeContext context)
    {
        var type = TypeVisitor.Visit(context.type());

        return ctx => new TypeAbbBind(type(ctx));
    }
}
