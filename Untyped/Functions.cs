using System;
using System.Collections.Immutable;
using Antlr4.Runtime;
using Common;
using Untyped.Terms;
using static Untyped.Core.Evaluation;
using static Untyped.Syntax.Printing;

namespace Untyped;

public static class Functions
{
    public static ITerm Eval(Context ctx, ITerm t) => Core.Evaluation.Eval(ctx, t);

    private static Func<Context, (ImmutableStack<ICommand>, Context)> Parse(string s)
    {
        if (string.IsNullOrWhiteSpace(s))
            throw new ArgumentException($"{nameof(s)} cannot be null or empty");

        var inputStream = new AntlrInputStream(s);
        var lexer = new TaplLexer(inputStream);
        var commonTokenStream = new CommonTokenStream(lexer);
        var parser = new TaplParser(commonTokenStream);
        var context = parser.toplevel();

        var visitor = new TopLevelVisitor();

        return visitor.Visit(context);
    }

    private static Context ProcessCommand(Context ctx, ICommand c)
    {
        switch (c)
        {
            case Eval e:
                var t = Eval(ctx, e.Term);
                Console.WriteLine(PrintTerm(ctx, t));
                return ctx;
            case Bind b:
                Console.WriteLine($"Bind {b.Name}");
                return ctx.AddBinding(b.Name, new NameBinding());
            default:
                throw new InvalidOperationException();
        }
    }

    public static Context Process(string source)
    {
        return CommandRunner.Process(source, Parse, ProcessCommand);
    }
}