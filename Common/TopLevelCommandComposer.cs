using System;
using System.Collections.Immutable;

namespace Common;

public static class TopLevelCommandComposer
{
    public static Func<Context, (ImmutableStack<ICommand>, Context)> Compose<TTopLevelContext, TCommandContext>(
        TTopLevelContext context,
        Func<TTopLevelContext, TTopLevelContext> getNext,
        Func<TTopLevelContext, TCommandContext> getCommand,
        Func<TTopLevelContext, Func<Context, (ImmutableStack<ICommand>, Context)>> visitNext,
        Func<TCommandContext, Func<Context, (ICommand, Context)>> getCommandFunc)
        where TTopLevelContext : class
        where TCommandContext : class
    {
        var next = getNext(context);
        Func<Context, (ImmutableStack<ICommand>, Context)> fnext =
            next is null ? c => (ImmutableStack<ICommand>.Empty, c) : visitNext(next);

        var commandContext = getCommand(context);
        if (commandContext is null)
            return fnext;

        var fcommand = getCommandFunc(commandContext);
        return ctx =>
        {
            var (command, ctx1) = fcommand(ctx);
            var (list, ctx2) = fnext(ctx1);
            return (list.Push(command), ctx2);
        };
    }
}