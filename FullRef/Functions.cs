using System;
using System.Collections.Immutable;
using Antlr4.Runtime;
using Common;
using FullRef.Core;
using FullRef.Syntax.Bindings;
using FullRef.Visitors;
using static FullRef.Syntax.Printing;

namespace FullRef;

public static class Functions
{
    public static Func<Context, (ImmutableStack<ICommand>, Context)> Parse(string s)
    {
        if (string.IsNullOrWhiteSpace(s))
            throw new ArgumentException($"{nameof(s)} cannot be null or empty");

        var inputStream = new AntlrInputStream(s);
        var lexer = new FullRefLexer(inputStream);
        var commonTokenStream = new CommonTokenStream(lexer);
        var parser = new FullRefParser(commonTokenStream);
        var context = parser.toplevel();

        var visitor = new TopLevelVisitor();

        return visitor.Visit(context);
    }

    public static (Context Context, Store Store) ProcessCommand(Context ctx, Store store, ICommand c)
    {
        switch (c)
        {
            case Eval e:
                var type = Typing.TypeOf(ctx, e.Term);
                var (t, nextStore) = Evaluation.Eval(ctx, store, e.Term);
                PrintTmATermWithType(ctx, t, type);
                return (ctx, nextStore);
            case Bind b:
                var b1 = CheckBinding(ctx, b.Binding);
                var (b2, nextBindingStore) = Evaluation.EvalBinding(ctx, store, b1);
                PrintBindingType(ctx, b2, b.Name);
                return (ctx.AddBinding(b.Name, b2), nextBindingStore.Shift(1));
            default:
                throw new InvalidOperationException();
        }
    }

    public static Context ProcessCommand(Context ctx, ICommand c) => ProcessCommand(ctx, Store.Empty, c).Context;

    private static IBinding CheckBinding(Context ctx, IBinding bind) => bind switch
    {
        NameBinding => bind,
        TypeVarBind => bind,
        TermAbbBind { Type: null } tab => new TermAbbBind(tab.Term, Typing.TypeOf(ctx, tab.Term)),
        TermAbbBind tab when Typing.TypeEqual(ctx, tab.Type, Typing.TypeOf(ctx, tab.Term)) => bind,
        TermAbbBind => throw new Exception("type of binding doesn't match declared type in " + bind),
        VarBind => bind,
        TypeAbbBind => bind,
        _ => throw new InvalidOperationException()
    };

    public static Context Process(string source)
    {
        if (string.IsNullOrWhiteSpace(source))
            throw new ArgumentException($"{nameof(source)} cannot be null or empty");

        var parse = Parse(source);
        var (commands, _) = parse(new Context());

        var ctx = new Context();
        var store = Store.Empty;
        foreach (var command in commands)
        {
            (ctx, store) = ProcessCommand(ctx, store, command);
        }

        return ctx;
    }
}