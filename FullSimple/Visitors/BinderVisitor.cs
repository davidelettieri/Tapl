using Antlr4.Runtime.Misc;
using Common;
using FullSimple.Syntax.Bindings;
using System;

namespace FullSimple.Visitors;

public class BinderVisitor : FullSimpleBaseVisitor<Func<Context, IBinding>>
{
    private static readonly TermVisitor _termVisitor = new TermVisitor();
    private static readonly TypeVisitor _typeVisitor = new TypeVisitor();

    public override Func<Context, IBinding> VisitBinder_term([NotNull] FullSimpleParser.Binder_termContext context)
    {
            var term = _termVisitor.Visit(context.term());

            return ctx => new TermAbbBind(term(ctx), null);
        }

    public override Func<Context, IBinding> VisitBinder_type([NotNull] FullSimpleParser.Binder_typeContext context)
    {
            var type = _typeVisitor.Visit(context.type());

            return ctx => new VarBind(type(ctx));
        }
}