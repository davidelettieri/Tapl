using Chapter7.Terms;
using Common;
using System;
using System.Linq;
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

        private static (Context, string) PickFreshName(Context ctx, string boundedVariable)
        {
            var count = ctx.Count(p => p.Item1.StartsWith(boundedVariable));

            if (count == 0)
            {
                return (ctx.Add((boundedVariable, new Binding())), boundedVariable);
            }

            var xp = boundedVariable + new string('\'', count);
            return (ctx.Add((xp, new Binding())), xp);
        }
    }
}
