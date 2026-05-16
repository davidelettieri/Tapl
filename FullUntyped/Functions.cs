using System;
using System.Collections.Immutable;
using System.Text.RegularExpressions;
using Antlr4.Runtime;
using Common;
using FullUntyped.Syntax.Bindings;
using FullUntyped.Visitors;
using static FullUntyped.Core.Evaluation;
using static FullUntyped.Syntax.Printing;

namespace FullUntyped;

public static class Functions
{
    private static readonly Regex UntypedLambdaPattern = new(@"\blambda\s+([a-z][a-zA-Z0-9]*|_)\s*\.",
        RegexOptions.Compiled);

    private static readonly Regex SlashBindingPattern = new(@"(?m)\b([a-z][a-zA-Z0-9]*)\s*/\s*;",
        RegexOptions.Compiled);

    public static Func<Context, (ImmutableStack<ICommand>, Context)> Parse(string s)
    {
        if (string.IsNullOrWhiteSpace(s))
            throw new ArgumentException($"{nameof(s)} cannot be null or empty");

        var normalizedSource = NormalizeUntypedSource(s);
        var inputStream = new AntlrInputStream(normalizedSource);
        var lexer = new FullUntypedLexer(inputStream);
        var commonTokenStream = new CommonTokenStream(lexer);
        var parser = new FullUntypedParser(commonTokenStream);
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
                PrintTmATerm(ctx, t);
                Console.WriteLine();
                return ctx;
            case Bind b:
                var b2 = EvalBinding(ctx, b.Binding);
                PrintBindingType(ctx, b2, b.Name);
                return ctx.AddBinding(b.Name, b2);
            default:
                throw new InvalidOperationException();
        }
    }

    private static IBinding EvalBinding(Context ctx, IBinding bind) => bind switch
    {
        TermAbbBind tab => new TermAbbBind(Eval(ctx, tab.Term), null),
        VarBind => new NameBinding(),
        _ => bind
    };

    private static string NormalizeUntypedSource(string source)
    {
        var withNamedBindings = SlashBindingPattern.Replace(source, "$1:Bool;");
        return UntypedLambdaPattern.Replace(withNamedBindings, "lambda $1:Bool.");
    }

    public static Context Process(string source) => CommandRunner.Process(source, Parse, ProcessCommand);
}
