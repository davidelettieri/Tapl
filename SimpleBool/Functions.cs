using Common;
using System;
using static SimpleBool.Syntax.Printing;
using static SimpleBool.Core.Evaluation;
using System.Collections.Immutable;
using Antlr4.Runtime;
using System.Linq;

namespace SimpleBool;

public static class Functions
{
    public static Func<Context, (ImmutableStack<ICommand>, Context)> Parse(string s)
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


    public static Context ProcessCommand(Context ctx, ICommand c)
    {
            switch (c)
            {
                case Eval e:
                    var t = Eval(ctx, e.Term);
                    PrintTerm(ctx, t);
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
            var fcommands = Parse(source);
            var commands = fcommands(new Context());

            return commands.Item1.Aggregate(new Context(), ProcessCommand);
        }
}