using System;
using System.Collections.Immutable;

namespace Common;

public static class CommandRunner
{
    public static Context Process(
        string source,
        Func<string, Func<Context, (ImmutableStack<ICommand>, Context)>> parse,
        Func<Context, ICommand, Context> processCommand)
    {
        if (string.IsNullOrWhiteSpace(source))
            throw new ArgumentException($"{nameof(source)} cannot be null or empty");

        var fcommands = parse(source);
        var (commands, _) = fcommands(new Context());

        var ctx = new Context();
        foreach (var command in commands)
        {
            ctx = processCommand(ctx, command);
        }

        return ctx;
    }
}