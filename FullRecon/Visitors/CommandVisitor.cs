using System;
using Common;
using FullRecon.Syntax.Bindings;

namespace FullRecon.Visitors;

public sealed class CommandVisitor : FullReconBaseVisitor<Func<Context, (ICommand, Context)>>
{
    private static readonly BinderVisitor BinderVisitor = new();
    private static readonly TermVisitor TermVisitor = new();

    public override Func<Context, (ICommand, Context)> VisitCommand_term(
        FullReconParser.Command_termContext context)
    {
        var info = context.GetFileInfo();
        var term = TermVisitor.Visit(context.term());
        return ctx => (new Eval(info, term(ctx)), ctx);
    }

    public override Func<Context, (ICommand, Context)> VisitCommand_binder(
        FullReconParser.Command_binderContext context)
    {
        var info = context.GetFileInfo();
        var bind = BinderVisitor.Visit(context.binder());
        var name = context.LCID().GetText();
        return ctx => (new Bind(info, name, bind(ctx)), ctx.AddName(name));
    }
}
