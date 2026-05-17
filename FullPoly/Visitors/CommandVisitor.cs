using System;
using Common;
using FullPoly.Syntax;

namespace FullPoly.Visitors;

public sealed class CommandVisitor : FullPolyBaseVisitor<Func<Context, (ICommand, Context)>>
{
    private static readonly BinderVisitor BinderVisitor = new();
    private static readonly TyBinderVisitor TypeBinderVisitor = new();
    private static readonly TermVisitor TermVisitor = new();

    public override Func<Context, (ICommand, Context)> VisitCommand_binder(
        FullPolyParser.Command_binderContext context)
    {
        var info = context.GetFileInfo();
        var bind = BinderVisitor.Visit(context.binder());
        var name = context.LCID().GetText();

        return ctx => (new Bind(info, name, bind(ctx)), ctx.AddName(name));
    }

    public override Func<Context, (ICommand, Context)> VisitCommand_term(
        FullPolyParser.Command_termContext context)
    {
        var info = context.GetFileInfo();
        var term = TermVisitor.Visit(context.term());
        return ctx => (new Eval(info, term(ctx)), ctx);
    }

    public override Func<Context, (ICommand, Context)> VisitCommand_tybinder(
        FullPolyParser.Command_tybinderContext context)
    {
        var info = context.GetFileInfo();
        var bind = TypeBinderVisitor.Visit(context.tybinder());
        var name = context.UCID().GetText();

        return ctx => (new Bind(info, name, bind(ctx)), ctx.AddName(name));
    }

    public override Func<Context, (ICommand, Context)> VisitCommand_somebind(
        FullPolyParser.Command_somebindContext context)
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
}
