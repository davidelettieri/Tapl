using Antlr4.Runtime.Misc;
using LetExercise.Syntax;
using Common;
using System;
using System.Collections.Immutable;

namespace LetExercise;

public class TopLevelVisitor : TaplBaseVisitor<Func<Context, (ImmutableStack<ICommand>, Context)>>
{
    private readonly TermVisitor _termVisitor = new TermVisitor();

    public override Func<Context, (ImmutableStack<ICommand>, Context)> VisitToplevel([NotNull] TaplParser.ToplevelContext context)
    {
            Func<Context, (ImmutableStack<ICommand>, Context)> fnext =
                context.toplevel() != null ? Visit(context.toplevel()) : c => (ImmutableStack.Create<ICommand>(), c);

            if (context.command() is null)
                return fnext;

            Func<Context, (ICommand, Context)> fcommand = GetCommand(context.command());
            return ctx =>
            {
                var (command, ctx1) = fcommand(ctx);
                var (list, ctx2) = fnext(ctx1);
                return (list.Push(command), ctx2);
            };
        }

    private Func<Context, (ICommand, Context)> GetCommand(TaplParser.CommandContext context)
    {
            var info = context.GetFileInfo();
            if (context.bind() != null)
            {
                var boundName = context.bind().VAR().GetText();
                return ctx => (new Bind(info, boundName, new NameBinding()), ctx.AddName(boundName));
            }

            var termFunc = _termVisitor.Visit(context.term());

            return ctx => (new Eval(info, termFunc(ctx)), ctx);
        }
}

public class TermVisitor : TaplBaseVisitor<Func<Context, ITerm>>
{
    private readonly TypeVisitor _typeVisitor = new TypeVisitor();

    public override Func<Context, ITerm> VisitAbs([NotNull] TaplParser.AbsContext context)
    {
            var boundVar = context.VAR().GetText();
            var body = Visit(context.term());
            var info = context.GetFileInfo();
            ITerm result(Context c) => new Abs(info, body(c.AddName(boundVar)), boundVar, _typeVisitor.Visit(context.type()));
            return result;
        }

    public override Func<Context, ITerm> VisitApp([NotNull] TaplParser.AppContext context)
    {
            var terms = context.term();
            var left = Visit(terms[0]);
            var right = Visit(terms[1]);
            var info = context.GetFileInfo();
            return c => new App(info, left(c), right(c));
        }

    public override Func<Context, ITerm> VisitFalse([NotNull] TaplParser.FalseContext context)
    {
            var info = context.GetFileInfo();
            return _ => new False(info);
        }

    public override Func<Context, ITerm> VisitIft([NotNull] TaplParser.IftContext context)
    {
            var terms = context.term();
            var condition = Visit(terms[0]);
            var then = Visit(terms[1]);
            var @else = Visit(terms[2]);
            var info = context.GetFileInfo();
            return c => new If(info, condition(c), then(c), @else(c));
        }

    public override Func<Context, ITerm> VisitLet([NotNull] TaplParser.LetContext context)
    {
            var variable = context.VAR().GetText();
            var letTerm = Visit(context.term()[0]);
            var inTerm = Visit(context.term()[1]);
            var info = context.GetFileInfo();

            return c => new Let(info, variable, letTerm(c), inTerm(c.AddName(variable)));
        }

    public override Func<Context, ITerm> VisitPar([NotNull] TaplParser.ParContext context)
    {
            return Visit(context.term());
        }

    public override Func<Context, ITerm> VisitTrue([NotNull] TaplParser.TrueContext context)
    {
            var info = context.GetFileInfo();
            return _ => new True(info);
        }

    public override Func<Context, ITerm> VisitVar([NotNull] TaplParser.VarContext context)
    {
            var name = context.VAR().GetText();
            var info = context.GetFileInfo();
            return ctx => new Var(info, ctx.NameToIndex(name), ctx.Length);
        }
}

public class TypeVisitor : TaplBaseVisitor<IType>
{
    public override IType VisitArrow([NotNull] TaplParser.ArrowContext context)
    {
            var el = context.type();
            return new TypeArrow(Visit(el[0]), Visit(el[1]));
        }

    public override IType VisitBool([NotNull] TaplParser.BoolContext context)
    {
            return new TypeBool();
        }
}