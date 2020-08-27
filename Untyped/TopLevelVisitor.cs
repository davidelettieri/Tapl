using Antlr4.Runtime.Misc;
using Untyped.Terms;
using Common;
using System;
using System.Collections.Immutable;

namespace Untyped
{
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
            if (context.bind() != null)
            {
                var info = context.GetFileInfo();
                var boundName = context.bind().VAR().GetText();
                return ctx => (new Bind(info, boundName, new NameBinding()), ctx.AddName(boundName));
            }

            var termFunc = _termVisitor.Visit(context.term());

            return ctx => (new Eval(context.GetFileInfo(), termFunc(ctx)), ctx);
        }
    }

    public class TermVisitor : TaplBaseVisitor<Func<Context, ITerm>>
    {
        public override Func<Context, ITerm> VisitAbs([NotNull] TaplParser.AbsContext context)
        {
            var boundVar = context.VAR().GetText();
            var body = Visit(context.term());
            ITerm result(Context c) => new Abs(body(c.AddName(boundVar)), boundVar);
            return result;
        }

        public override Func<Context, ITerm> VisitApp([NotNull] TaplParser.AppContext context)
        {
            var terms = context.term();
            var left = Visit(terms[0]);
            var right = Visit(terms[1]);

            return c => new App(left(c), right(c));
        }

        public override Func<Context, ITerm> VisitPar([NotNull] TaplParser.ParContext context)
        {
            return Visit(context.term());
        }

        public override Func<Context, ITerm> VisitVar([NotNull] TaplParser.VarContext context)
        {
            var name = context.VAR().GetText();
            return ctx => new Var(ctx.NameToIndex(name), ctx.Length);
        }
    }
}
