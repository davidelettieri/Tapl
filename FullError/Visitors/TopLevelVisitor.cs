using System;
using System.Collections.Immutable;
using Common;

namespace FullError.Visitors;

public sealed class TopLevelVisitor : FullErrorBaseVisitor<Func<Context, (ImmutableStack<ICommand>, Context)>>
{
    private static readonly CommandVisitor CommandVisitor = new();

    public override Func<Context, (ImmutableStack<ICommand>, Context)> VisitToplevel(
        FullErrorParser.ToplevelContext context)
    {
        return TopLevelCommandComposer.Compose(
            context,
            c => c.toplevel(),
            c => c.command(),
            Visit,
            c => CommandVisitor.Visit(c));
    }
}
