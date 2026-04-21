using Antlr4.Runtime.Misc;
using Common;
using FullSimple.Syntax.Bindings;
using System;

namespace FullSimple.Visitors;

public sealed class TyBinderVisitor : FullSimpleBaseVisitor<Func<Context, IBinding>>
{
    private static readonly TypeVisitor TypeVisitor = new();

    public override Func<Context, IBinding> VisitTybinder_type(FullSimpleParser.Tybinder_typeContext context)
    {
        if (context.type() != null)
        {
            var type = TypeVisitor.Visit(context.type());

            return ctx => new TypeAbbBind(type(ctx));
        }

        return _ => new TypeVarBind();
    }
}