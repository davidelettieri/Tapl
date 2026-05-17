using System;
using System.Collections.Immutable;
using Antlr4.Runtime;
using Common;
using FullPoly.Core;
using FullPoly.Syntax;
using FullPoly.Syntax.Bindings;
using FullPoly.Syntax.Terms;
using FullPoly.Visitors;
using static FullPoly.Core.Evaluation;
using static FullPoly.Syntax.Printing;

namespace FullPoly;

public static class Functions
{
    public static Func<Context, (ImmutableStack<ICommand>, Context)> Parse(string s)
    {
        if (string.IsNullOrWhiteSpace(s))
            throw new ArgumentException($"{nameof(s)} cannot be null or empty");

        var inputStream = new AntlrInputStream(s);
        var lexer = new FullPolyLexer(inputStream);
        var commonTokenStream = new CommonTokenStream(lexer);
        var parser = new FullPolyParser(commonTokenStream);
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
            case SomeBindCommand sb:
                var t2 = Evaluation.Eval(ctx, sb.Term);
                var lcst = Typing.TypeOf(ctx, sb.Term);
                if (lcst is TypeSome some)
                {
                    IBinding bind = t2 is Pack pack
                        ? new TermAbbBind(pack.Term, Substitution.TypeSubsTop(pack.WitnessType, some.Body))
                        : new VarBind(some.Body);
                    var ctx1 = ctx.AddBinding(sb.TypeVar, new TypeVarBind());
                    PrintBindingType(ctx1, bind, sb.Var);
                    return ctx1.AddBinding(sb.Var, bind);
                }

                throw new Exception("existential type expected");
            default:
                throw new InvalidOperationException();
        }
    }

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

    private static IBinding EvalBinding(Context ctx, IBinding bind) => bind switch
    {
        TermAbbBind tab => new TermAbbBind(Eval(ctx, tab.Term), tab.Type),
        _ => bind
    };

    public static Context Process(string source) => CommandRunner.Process(source, Parse, ProcessCommand);
}
