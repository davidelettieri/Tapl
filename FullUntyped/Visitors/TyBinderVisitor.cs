using System;
using Common;
using FullUntyped.Syntax.Bindings;

namespace FullUntyped.Visitors;

public sealed class TyBinderVisitor : FullUntypedBaseVisitor<Func<Context, IBinding>>
{
    private static readonly TypeVisitor TypeVisitor = new();

    public override Func<Context, IBinding> VisitTybinder_type(FullUntypedParser.Tybinder_typeContext context)
    {
        if (context.type() != null)
        {
            var type = TypeVisitor.Visit(context.type());

            return ctx => new TypeAbbBind(type(ctx));
        }

        return _ => new TypeVarBind();
    }
}