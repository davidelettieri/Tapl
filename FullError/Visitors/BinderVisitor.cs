using System;
using Common;
using FullError.Syntax.Bindings;

namespace FullError.Visitors;

public sealed class BinderVisitor : FullErrorBaseVisitor<Func<Context, IBinding>>
{
    private static readonly TermVisitor TermVisitor = new();
    private static readonly TypeVisitor TypeVisitor = new();

    public override Func<Context, IBinding> VisitBinder_term(FullErrorParser.Binder_termContext context)
    {
        var term = TermVisitor.Visit(context.term());

        return ctx => new TermAbbBind(term(ctx), null);
    }

    public override Func<Context, IBinding> VisitBinder_type(FullErrorParser.Binder_typeContext context)
    {
        var type = TypeVisitor.Visit(context.type());

        return ctx => new VarBind(type(ctx));
    }
}
