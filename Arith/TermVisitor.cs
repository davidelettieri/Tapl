using Arith.Terms;
using Common;

namespace Arith;

public class TermVisitor : TaplBaseVisitor<ITerm>
{
    public override ITerm VisitFalse(TaplParser.FalseContext context) => new False();

    public override ITerm VisitIfThenElse(TaplParser.IfThenElseContext context)
    {
        var t = context.term();

        return new If(Visit(t[0]), Visit(t[1]), Visit(t[2]));
    }

    public override ITerm VisitIsZero(TaplParser.IsZeroContext context)
    {
        var of = context.term();
        return new IsZero(Visit(of));
    }

    public override ITerm VisitPar(TaplParser.ParContext context) => Visit(context.term());

    public override ITerm VisitPred(TaplParser.PredContext context)
    {
        var of = context.term();
        return new Pred(Visit(of));
    }

    public override ITerm VisitSucc(TaplParser.SuccContext context)
    {
        var of = context.term();
        return new Succ(Visit(of));
    }

    public override ITerm VisitTrue(TaplParser.TrueContext context) => new True();

    public override ITerm VisitZero(TaplParser.ZeroContext context) => new Zero();
}