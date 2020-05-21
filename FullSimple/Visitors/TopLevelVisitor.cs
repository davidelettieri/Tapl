using Antlr4.Runtime;
using Antlr4.Runtime.Misc;
using FullSimple.Syntax;
using Common;
using System;
using System.Collections.Immutable;
using FullSimple.Syntax.Terms;
using FullSimple.Syntax.Types;
using static FullSimple.Helper;
using FullSimple.Command;

namespace FullSimple.Visitors
{
    public class TopLevelVisitor : FullSimpleBaseVisitor<Func<Context, (ImmutableStack<ICommand>, Context)>>
    {
        private static readonly TermVisitor _termVisitor = new TermVisitor();
        private static readonly TypeVisitor _typeVisitor = new TypeVisitor();

        public override Func<Context, (ImmutableStack<ICommand>, Context)> VisitToplevel([NotNull] FullSimpleParser.ToplevelContext context)
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

        private Func<Context, (ICommand, Context)> GetCommand(FullSimpleParser.CommandContext context)
        {
            if (context.term() != null)
            {
                var termFunc = _termVisitor.Visit(context.term());
                return ctx => (new Eval(termFunc(ctx)), ctx);
            }

            if (context.UCID() != null && context.tybinder() != null)
            {
                var id = context.UCID().GetText();
                var type = _typeVisitor.Visit(context.binder().type());

                return ctx => (new Binder(id, new TypeBind type))
            }
        }
    }

    public class TypeVisitor : FullSimpleBaseVisitor<Func<Context, IType>>
    {

    }
}
