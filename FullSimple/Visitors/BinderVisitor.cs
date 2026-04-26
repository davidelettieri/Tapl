using System;
using Common;
using FullSimple.Syntax.Bindings;

namespace FullSimple.Visitors;

public sealed class BinderVisitor : FullSimpleBaseVisitor<Func<Context, IBinding>>
{
    private static readonly TermVisitor TermVisitor = new();
    private static readonly TypeVisitor TypeVisitor = new();

    public override Func<Context, IBinding> VisitBinder_term(FullSimpleParser.Binder_termContext context)
    {
        var term = TermVisitor.Visit(context.term());

        return ctx => new TermAbbBind(term(ctx), null);
    }

    public override Func<Context, IBinding> VisitBinder_type(FullSimpleParser.Binder_typeContext context)
    {
        var type = TypeVisitor.Visit(context.type());

        return ctx => new VarBind(type(ctx));
    }
}