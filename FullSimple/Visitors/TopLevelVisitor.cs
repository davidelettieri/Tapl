using System;
using System.Collections.Immutable;
using Common;

namespace FullSimple.Visitors;

public sealed class TopLevelVisitor : FullSimpleBaseVisitor<Func<Context, (ImmutableStack<ICommand>, Context)>>
{
    private static readonly CommandVisitor CommandVisitor = new();

    public override Func<Context, (ImmutableStack<ICommand>, Context)> VisitToplevel_command(
        FullSimpleParser.Toplevel_commandContext context)
    {
        var command = CommandVisitor.Visit(context.command());
        var commands = Visit(context.toplevel());

        return ctx =>
        {
            var (cmd, ctx1) = command(ctx);
            var (cmds, ctx2) = commands(ctx1);
            return (cmds.Push(cmd), ctx2);
        };
    }

    public override Func<Context, (ImmutableStack<ICommand>, Context)> VisitToplevel_eof(
        FullSimpleParser.Toplevel_eofContext context) => ctx => (ImmutableStack<ICommand>.Empty, ctx);
}