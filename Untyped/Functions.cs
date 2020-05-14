using Antlr4.Runtime;
using Common;
using System;
using System.Collections.Immutable;
using System.Linq;
using Untyped.Terms;

namespace Untyped
{
    public static class Functions
    {
        #region Core

        public static bool IsVal(ITerm t) => t is Abs;

        private static ITerm eval1(Context ctx, ITerm t)
        {
            return t switch
            {
                App app when app.Left is Abs abs && IsVal(app.Right) => TermSubsTop(app.Right, abs.Body),
                App app when IsVal(app.Left) => new App(app.Left, eval1(ctx, app.Right)),
                App app => new App(eval1(ctx, app.Left), app.Right),
                _ => throw new NoRulesAppliesException()
            };
        }

        public static ITerm Eval(Context ctx, ITerm t)
        {
            try
            {
                var t1 = eval1(ctx, t);

                return Eval(ctx, t1);
            }
            catch (NoRulesAppliesException)
            {
                return t;
            }
        }

        #endregion

        public static ITerm TmMap(Func<int, Var, ITerm> onVar, int c, ITerm t)
        {
            ITerm walk(int c, ITerm t)
            {
                return t switch
                {
                    Var var => onVar(c, var),
                    Abs abs => new Abs(walk(c + 1, abs.Body), abs.BoundedVariable),
                    App app => new App(walk(c, app.Left), walk(c, app.Right)),
                    _ => throw new InvalidOperationException()
                };
            }

            return walk(c, t);
        }

        public static ITerm TermShiftAbove(int d, int c, ITerm t)
        {
            ITerm onVar(int c, Var v)
            {
                if (v.Index >= c) return new Var(v.Index + d, v.ContextLength + d);

                return new Var(v.Index, v.ContextLength + d);
            }

            return TmMap(onVar, c, t);
        }

        public static ITerm TermShift(int d, ITerm t) => TermShiftAbove(d, 0, t);

        /// <summary>
        /// Substitution: [j -> s]
        /// </summary>
        /// <param name="j">Variable to be sustituted</param>
        /// <param name="s"></param>
        /// <param name="t"></param>
        /// <returns></returns>
        public static ITerm TermSubst(int j, ITerm s, ITerm t)
        {
            ITerm onVar(int c, Var v) => v.Index == j + c ? TermShift(c, s) : v;

            return TmMap(onVar, 0, t);
        }

        public static ITerm TermSubsTop(ITerm s, ITerm t)
            => TermShift(-1, TermSubst(0, TermShift(1, s), t));

        public static string PrintTerm(Context ctx, ITerm t)
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
                    else
                        return "[bad index]";
                default:
                    throw new InvalidOperationException();
            }
        }

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
}
