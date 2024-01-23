using Antlr4.Runtime;
using Common;
using System;
using System.Collections.Immutable;
using System.Linq;
using Untyped.Terms;

namespace Untyped;

public static class Functions
{
    #region Core

    private static bool IsVal(ITerm t) => t is Abs;

    private static ITerm Eval1(Context ctx, ITerm t)
    {
        return t switch
        {
            App { Left: Abs abs } app when IsVal(app.Right) => TermSubsTop(app.Right, abs.Body),
            App app when IsVal(app.Left) => new App(app.Left, Eval1(ctx, app.Right)),
            App app => new App(Eval1(ctx, app.Left), app.Right),
            _ => throw new NoRulesAppliesException()
        };
    }

    public static ITerm Eval(Context ctx, ITerm t)
    {
        try
        {
            var t1 = Eval1(ctx, t);

            return Eval(ctx, t1);
        }
        catch (NoRulesAppliesException)
        {
            return t;
        }
    }

    #endregion

    private static ITerm TmMap(Func<int, Var, ITerm> onVar, int c, ITerm t)
    {
        ITerm Walk(int c, ITerm t)
        {
            return t switch
            {
                Var var => onVar(c, var),
                Abs abs => new Abs(Walk(c + 1, abs.Body), abs.BoundedVariable),
                App app => new App(Walk(c, app.Left), Walk(c, app.Right)),
                _ => throw new InvalidOperationException()
            };
        }

        return Walk(c, t);
    }

    private static ITerm TermShiftAbove(int d, int c, ITerm t)
    {
        ITerm OnVar(int c, Var v)
        {
            if (v.Index >= c) return new Var(v.Index + d, v.ContextLength + d);

            return new Var(v.Index, v.ContextLength + d);
        }

        return TmMap(OnVar, c, t);
    }

    private static ITerm TermShift(int d, ITerm t) => TermShiftAbove(d, 0, t);

    /// <summary>
    /// Substitution: [j -> s]
    /// </summary>
    /// <param name="j">Variable to be substituted</param>
    /// <param name="s"></param>
    /// <param name="t"></param>
    /// <returns></returns>
    private static ITerm TermSubst(int j, ITerm s, ITerm t)
    {
        ITerm OnVar(int c, Var v) => v.Index == j + c ? TermShift(c, s) : v;

        return TmMap(OnVar, 0, t);
    }

    private static ITerm TermSubsTop(ITerm s, ITerm t)
        => TermShift(-1, TermSubst(0, TermShift(1, s), t));

    private static string PrintTerm(Context ctx, ITerm t)
    {
        switch (t)
        {
            case Abs abs:
                var (ctxp, xp) = ctx.PickFreshName(abs.BoundedVariable);
                return $"(lambda {xp}.{PrintTerm(ctxp, abs.Body)})";
            case App app:
                return $"({PrintTerm(ctx, app.Left)}{PrintTerm(ctx, app.Right)})";
            case Var var:
                if (ctx.Length == var.ContextLength)
                    return ctx.IndexToName(var.Index);
                return "[bad index]";
            default:
                throw new InvalidOperationException();
        }
    }

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
        var fcommands = Parse(source);
        var commands = fcommands(new Context());

        return commands.Item1.Aggregate(new Context(), ProcessCommand);
    }
}