using Antlr4.Runtime.Misc;
using Chapter7.Terms;
using Common;

namespace Chapter7
{
    public class TermVisitor : TaplBaseVisitor<ITerm>
    {
        public override ITerm VisitAbs([NotNull] TaplParser.AbsContext context)
        {
            return new Abs(Visit(context.term()), context.VAR().GetText());
        }

        public override ITerm VisitApp([NotNull] TaplParser.AppContext context)
        {
            return new App(Visit(context.term()[0]), Visit(context.term()[1]));
        }

        public override ITerm VisitPar([NotNull] TaplParser.ParContext context)
        {
            return Visit(context.term());
        }

        public override ITerm VisitVar([NotNull] TaplParser.VarContext context)
        {
            return new Var(0, 0);
        }
    }
}
