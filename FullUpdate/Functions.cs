using System;
using System.Collections.Immutable;
using Common;
using FullUpdate.Core;
using FullUpdate.Syntax;
using FullUpdate.Syntax.Bindings;
using static FullUpdate.Core.Evaluation;
using static FullUpdate.Syntax.Printing;

namespace FullUpdate;

public static class Functions
{
    public static Func<Context, (ImmutableStack<ICommand>, Context)> Parse(string s)
    {
        if (string.IsNullOrWhiteSpace(s))
            throw new ArgumentException($"{nameof(s)} cannot be null or empty");

        var lexer = new Parser.Lexer(s);
        var tokens = lexer.Tokenize();
        var parser = new Parser.Parser(tokens);
        return parser.ParseToplevel();
    }

    public static Context ProcessCommand(Context ctx, ICommand c)
    {
        switch (c)
        {
            case EvalCommand e:
                var type = Typing.TypeOf(ctx, e.Term);
                var t = Eval(ctx, e.Term);
                PrintTmATerm(ctx, t);
                PrintType(ctx, type);
                return ctx;

            case BindCommand b:
                var b1 = Typing.CheckBinding(b.Info, ctx, b.Binding);
                var b2 = EvalBinding(ctx, b1);
                PrintBindingType(ctx, b2, b.Name);
                return ctx.AddBinding(b.Name, b2);

            case SomeBindCommand sb:
                var tyT = Typing.TypeOf(ctx, sb.Term);
                var lcst = Typing.Lcst(ctx, tyT);
                if (lcst is TypeSome some)
                {
                    var t2 = Eval(ctx, sb.Term);
                    IBinding bind = t2 is Syntax.Terms.Pack pack
                        ? new TermAbbBind(Shifting.TermShift(1, pack.Term), some.Body)
                        : new VarBind(some.Body);
                    var ctx1 = ctx.AddBinding(sb.TypeVar, new TypeVarBind(some.Bound));
                    var ctx2 = ctx1.AddBinding(sb.Var, bind);

                    Console.Write(sb.TypeVar);
                    Console.WriteLine();
                    Console.Write(sb.Var);
                    Console.Write(" : ");
                    var pp = new PrettyPrinter();
                    // printty ctx1 tyBody  -> prints body type in ctx1
                    // We use the printing helper via reflection of the Printing module
                    PrintBodyType(ctx1, some.Body);
                    Console.WriteLine();
                    return ctx2;
                }
                throw new TaplTypingException(sb.Info, "existential type expected");

            default:
                throw new InvalidOperationException();
        }
    }

    private static void PrintBodyType(Context ctx, IType t)
    {
        // Print type using the internal type printer
        var pp = new PrettyPrinter();
        FullUpdate.Syntax.Printing.PrintBodyType(pp, ctx, t);
        Console.Write(pp.ToString());
    }

    public static Context Process(string source) => CommandRunner.Process(source, Parse, ProcessCommand);
}
