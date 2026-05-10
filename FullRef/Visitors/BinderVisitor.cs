using System;
using Common;
using FullRef.Syntax.Bindings;

namespace FullRef.Visitors;

public sealed class BinderVisitor : FullRefBaseVisitor<Func<Context, IBinding>>
{
    private static readonly TermVisitor TermVisitor = new();
    private static readonly TypeVisitor TypeVisitor = new();

    public override Func<Context, IBinding> VisitBinder_term(FullRefParser.Binder_termContext context)
    {
        var term = TermVisitor.Visit(context.term());

        return ctx => new TermAbbBind(term(ctx), null);
    }

    public override Func<Context, IBinding> VisitBinder_type(FullRefParser.Binder_typeContext context)
    {
        var type = TypeVisitor.Visit(context.type());

        return ctx => new VarBind(type(ctx));
    }
}