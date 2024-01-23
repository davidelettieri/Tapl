using Antlr4.Runtime.Misc;
using Arith.Terms;
using Common;

namespace Arith;

public class TermVisitor : TaplBaseVisitor<ITerm>
{
    public override ITerm VisitFalse([NotNull] TaplParser.FalseContext context)
    {
        return new False();
    }

    public override ITerm VisitIfThenElse([NotNull] TaplParser.IfThenElseContext context)
    {
        var t = context.term();

        return new If(Visit(t[0]), Visit(t[1]), Visit(t[2]));
    }

    public override ITerm VisitIsZero([NotNull] TaplParser.IsZeroContext context)
    {
        var of = context.term();
        return new IsZero(Visit(of));
    }

    public override ITerm VisitPar([NotNull] TaplParser.ParContext context)
    {
        return Visit(context.term());
    }

    public override ITerm VisitPred([NotNull] TaplParser.PredContext context)
    {
        var of = context.term();
        return new Pred(Visit(of));
    }

    public override ITerm VisitSucc([NotNull] TaplParser.SuccContext context)
    {
        var of = context.term();
        return new Succ(Visit(of));
    }

    public override ITerm VisitTrue([NotNull] TaplParser.TrueContext context)
    {
        return new True();
    }

    public override ITerm VisitZero([NotNull] TaplParser.ZeroContext context)
    {
        return new Zero();
    }
}