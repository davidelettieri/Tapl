using Antlr4.Runtime.Misc;
using Common;
using System;
using System.Collections.Immutable;
using static FullSimple.Helper;

namespace FullSimple.Visitors
{
    public class BinderVisitor : FullSimpleBaseVisitor<Func<Context, IBinding>>
    {

    }

    public class CommandVisitor : FullSimpleBaseVisitor<Func<Context, (ICommand, Context)>>
    {
        private static readonly BinderVisitor _binderVisitor = new BinderVisitor();

        public override Func<Context, (ICommand, Context)> VisitCommand_binder([NotNull] FullSimpleParser.Command_binderContext context)
        {
            return base.VisitCommand_binder(context);
        }

        public override Func<Context, (ICommand, Context)> VisitCommand_term([NotNull] FullSimpleParser.Command_termContext context)
        {
            return base.VisitCommand_term(context);
        }

        public override Func<Context, (ICommand, Context)> VisitCommand_tybinder([NotNull] FullSimpleParser.Command_tybinderContext context)
        {
            var info = GetFileInfo(context);
            var bind = _binderVisitor.Visit(context.tybinder());
            var name = context.UCID().GetText()

            return ctx => (new Bind(info, name, bind(ctx)),
        }
    }
    public class TopLevelVisitor : FullSimpleBaseVisitor<Func<Context, (ImmutableStack<ICommand>, Context)>>
    {
        private static readonly TermVisitor _termVisitor = new TermVisitor();
        private static readonly TypeVisitor _typeVisitor = new TypeVisitor();
        private static readonly CommandVisitor _commandVisitor = new CommandVisitor();

        public override Func<Context, (ImmutableStack<ICommand>, Context)> VisitToplevel_command([NotNull] FullSimpleParser.Toplevel_commandContext context)
        {
            var command = _commandVisitor.Visit(context.command());
            var commands = Visit(context.toplevel());

            return ctx =>
            {
                var (cmd, ctx1) = command(ctx);
                var (cmds, ctx2) = commands(ctx1);
                return (cmds.Push(cmd), ctx2);
            };
        }

        public override Func<Context, (ImmutableStack<ICommand>, Context)> VisitToplevel_eof([NotNull] FullSimpleParser.Toplevel_eofContext context)
        {
            return ctx => (ImmutableStack<ICommand>.Empty, ctx);
        }

    }

    public class TypeVisitor : FullSimpleBaseVisitor<Func<Context, IType>>
    {

    }
}
