using Antlr4.Runtime.Misc;
using Common;
using System;
using System.Collections.Immutable;

namespace FullSimple.Visitors;

public class TopLevelVisitor : FullSimpleBaseVisitor<Func<Context, (ImmutableStack<ICommand>, Context)>>
{
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