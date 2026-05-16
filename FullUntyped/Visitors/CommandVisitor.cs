using System;
using Common;

namespace FullUntyped.Visitors;

public sealed class CommandVisitor : FullUntypedBaseVisitor<Func<Context, (ICommand, Context)>>
{
    private static readonly BinderVisitor BinderVisitor = new();
    private static readonly TyBinderVisitor TypeBinderVisitor = new();
    private static readonly TermVisitor TermVisitor = new();

    public override Func<Context, (ICommand, Context)> VisitCommand_binder(
        FullUntypedParser.Command_binderContext context)
    {
        var info = context.GetFileInfo();
        var bind = BinderVisitor.Visit(context.binder());
        var name = context.LCID().GetText();

        return ctx => (new Bind(info, name, bind(ctx)), ctx.AddName(name));
    }

    public override Func<Context, (ICommand, Context)> VisitCommand_term(
        FullUntypedParser.Command_termContext context)
    {
        var info = context.GetFileInfo();
        var term = TermVisitor.Visit(context.term());
        return ctx => (new Eval(info, term(ctx)), ctx);
    }

    public override Func<Context, (ICommand, Context)> VisitCommand_tybinder(
        FullUntypedParser.Command_tybinderContext context)
    {
        var info = context.GetFileInfo();
        var bind = TypeBinderVisitor.Visit(context.tybinder());
        var name = context.UCID().GetText();

        return ctx => (new Bind(info, name, bind(ctx)), ctx.AddName(name));
    }
}