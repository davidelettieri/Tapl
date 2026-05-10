using System;
using System.Collections.Immutable;
using Antlr4.Runtime;
using Common;
using FullError.Core;
using FullError.Syntax.Bindings;
using FullError.Visitors;
using static FullError.Core.Evaluation;
using static FullError.Syntax.Printing;

namespace FullError;

public static class Functions
{
    public static Func<Context, (ImmutableStack<ICommand>, Context)> Parse(string s)
    {
        if (string.IsNullOrWhiteSpace(s))
            throw new ArgumentException($"{nameof(s)} cannot be null or empty");

        var inputStream = new AntlrInputStream(s);
        var lexer = new FullErrorLexer(inputStream);
        var commonTokenStream = new CommonTokenStream(lexer);
        var parser = new FullErrorParser(commonTokenStream);
        var context = parser.toplevel();

        var visitor = new TopLevelVisitor();

        return visitor.Visit(context);
    }

    public static Context ProcessCommand(Context ctx, ICommand c)
    {
        switch (c)
        {
            case Eval e:
                var type = Typing.TypeOf(ctx, e.Term);
                var t = Eval(ctx, e.Term);
                PrintTmATerm(ctx, t);
                PrintType(ctx, type);
                return ctx;
            case Bind b:
                var b1 = CheckBinding(ctx, b.Binding);
                var b2 = EvalBinding(ctx, b1);
                PrintBindingType(ctx, b2, b.Name);
                return ctx.AddBinding(b.Name, b2);
            default:
                throw new InvalidOperationException();
        }
    }

    private static IBinding CheckBinding(Context ctx, IBinding bind) => bind switch
    {
        NameBinding => bind,
        TypeVarBind => bind,
        TermAbbBind { Type: null } tab => new TermAbbBind(tab.Term, Typing.TypeOf(ctx, tab.Term)),
        TermAbbBind tab when Typing.Subtype(ctx, Typing.TypeOf(ctx, tab.Term), tab.Type!) => bind,
        TermAbbBind => throw new Exception("type of binding doesn't match declared type in " + bind),
        VarBind => bind,
        TypeAbbBind => bind,
        _ => throw new InvalidOperationException()
    };

    public static Context Process(string source) => CommandRunner.Process(source, Parse, ProcessCommand);
}
