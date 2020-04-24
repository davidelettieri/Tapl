using Antlr4.Runtime.Misc;
using Chapter7.Terms;
using Common;
using System;

namespace Chapter7
{
    public class TermVisitor : TaplBaseVisitor<Func<Context, ITerm>>
    {
        public override Func<Context, ITerm> VisitAbs([NotNull] TaplParser.AbsContext context)
        {
            var boundVar = context.VAR().GetText();
            var body = Visit(context.term());
            ITerm result(Context c) => new Abs(body(c.Add(boundVar, new Binding())), boundVar);
            return result;
        }

        public override Func<Context, ITerm> VisitApp([NotNull] TaplParser.AppContext context)
        {
            var terms = context.term();
            var left = Visit(terms[0]);
            var right = Visit(terms[1]);

            return c => new App(left(c), right(c));
        }

        public override Func<Context, ITerm> VisitPar([NotNull] TaplParser.ParContext context)
        {
            return Visit(context.term());
        }

        public override Func<Context, ITerm> VisitVar([NotNull] TaplParser.VarContext context)
        {
            var name = context.VAR().GetText();
            return ctx => new Var(ctx.NameToIndex(name), 0);
        }
    }
}
