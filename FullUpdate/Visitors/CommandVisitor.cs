using System;
using Common;
using FullUpdate.Syntax;
using FullUpdate.Syntax.Bindings;

namespace FullUpdate.Visitors;

public sealed class CommandVisitor : FullUpdateBaseVisitor<Func<Context, (ICommand, Context)>>
{
    private static readonly BinderVisitor BinderVisitor = new();
    private static readonly TyBinderVisitor TyBinderVisitor = new();
    private static readonly TermVisitor TermVisitor = new();

    public override Func<Context, (ICommand, Context)> VisitCommand_somebind(
        FullUpdateParser.Command_somebindContext context)
    {
        var info = context.GetFileInfo();
        var tyX = context.UCID().GetText();
        var x = context.LCID().GetText();
        var term = TermVisitor.Visit(context.term());
        return ctx =>
        {
            var ctx1 = ctx.AddName(tyX);
            var ctx2 = ctx1.AddName(x);
            return (new SomeBindCommand(info, tyX, x, term(ctx)), ctx2);
        };
    }

    public override Func<Context, (ICommand, Context)> VisitCommand_tybinder(
        FullUpdateParser.Command_tybinderContext context)
    {
        var info = context.GetFileInfo();
        var name = context.UCID().GetText();
        var bind = TyBinderVisitor.Visit(context.tybinder());
        return ctx => (new BindCommand(info, name, bind(ctx)), ctx.AddName(name));
    }

    public override Func<Context, (ICommand, Context)> VisitCommand_binder(
        FullUpdateParser.Command_binderContext context)
    {
        var info = context.GetFileInfo();
        var name = context.LCID().GetText();
        var bind = BinderVisitor.Visit(context.binder());
        return ctx => (new BindCommand(info, name, bind(ctx)), ctx.AddName(name));
    }

    public override Func<Context, (ICommand, Context)> VisitCommand_term(
        FullUpdateParser.Command_termContext context)
    {
        var info = context.GetFileInfo();
        var term = TermVisitor.Visit(context.term());
        return ctx => (new EvalCommand(info, term(ctx)), ctx);
    }
}
