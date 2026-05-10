using System;
using Common;
using FullRef.Syntax.Bindings;

namespace FullRef.Visitors;

public sealed class TyBinderVisitor : FullRefBaseVisitor<Func<Context, IBinding>>
{
    private static readonly TypeVisitor TypeVisitor = new();

    public override Func<Context, IBinding> VisitTybinder_type(FullRefParser.Tybinder_typeContext context)
    {
        if (context.type() != null)
        {
            var type = TypeVisitor.Visit(context.type());

            return ctx => new TypeAbbBind(type(ctx));
        }

        return _ => new TypeVarBind();
    }
}