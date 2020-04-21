using Chapter7.Terms;
using Common;
using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Context = System.Collections.Immutable.ImmutableList<(string, Chapter7.Binding)>;
namespace Chapter7
{
    public static class Functions
    {
        public static string PrintTerm(Context ctx, ITerm t)
        {
            switch (t)
            {
                case Abs abs:
                    var (ctxp, xp) = PickFreshName(ctx, abs.BoundedVariable);
                    return $"(lambda {xp}.{PrintTerm(ctxp, abs.Body)}";
                case App app:
                    return $"({PrintTerm(ctx, app.Left)}{PrintTerm(ctx, app.Right)}";
                case Var var:
                    if (ctx.Count == var.ContextLength)
                        return ctx[var.ContextLength - 1].Item1;
                    else
                        return "[bad index]";
                default:
                    throw new InvalidOperationException();
            };
        }

        public static (Context, string) PickFreshName(Context ctx, string boundedVariable)
        {
            var count = ctx.Count(p => p.Item1.StartsWith(boundedVariable));

            if (count == 0)
            {
                return (ctx.Add((boundedVariable, new Binding())), boundedVariable);
            }

            var xp = boundedVariable + new string('\'', count);
            return (ctx.Add((xp, new Binding())), xp);
        }

        public static ITerm Substitution(int variable, ITerm s, ITerm t)
        {
            return t switch
            {
                Var var => var.Index == variable ? s : var,
                Abs abs => new Abs(Substitution(variable + 1, s, abs.Body), abs.BoundedVariable),
                App app => new App(Substitution(variable, s, app.Left), Substitution(variable, s, app.Right)),
                _ => throw new InvalidOperationException()
            };
        }

        private static ITerm TermSubstitutionTop(ITerm s, ITerm t) => Shift(-1, Substitution(0, Shift(1, s), t));

        public static ITerm Shift(int d, ITerm t)
        {
            return Walk(t, 0);

            ITerm Walk(ITerm t, int cutoff)
            {
                switch (t)
                {
                    case Abs abs:
                        return new Abs(Walk(abs.Body, cutoff + 1), abs.BoundedVariable);
                    case App app:
                        return new App(Walk(app.Left, cutoff), Walk(app.Right, cutoff));
                    case Var var:
                        if (var.Index < cutoff)
                            return new Var(var.Index, var.ContextLength + d);

                        return new Var(var.Index + d, var.ContextLength + d);
                    default:
                        throw new InvalidOperationException();
                };
            }
        }

        public static bool IsVal(ITerm t) => t is Abs;


        private static ITerm _eval(Context ctx, ITerm t)
        {
            return t switch
            {
                App app when app.Left is Abs abs && IsVal(app.Right) => TermSubstitutionTop(app.Right, abs.Body),
                App app when IsVal(app.Left) => new App(app.Left, _eval(ctx, app.Right)),
                App app => new App(_eval(ctx, app.Left), app.Right),
                _ => throw new NoRulesAppliesException()
            };
        }

        public static ITerm Eval(Context ctx, ITerm t)
        {
            try
            {
                var t1 = _eval(ctx, t);

                return Eval(ctx, t1);
            }
            catch (NoRulesAppliesException)
            {
                return t;
            }
        }

        //private static ITerm _eval(ITerm t)
        //{
        //    t switch
        //    {
        //        App app when !IsVal(app.Left) => new App(_eval(app.Left), app.Right, app.ContextLength),
        //        App app when IsVal(app.Left) && !IsVal(app.Right) => new App(app.Left, _eval(app.Right), app.ContextLength),
        //        App app =>
        //    }
        //}
    }
}
