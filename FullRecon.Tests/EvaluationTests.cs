using Common;
using FullRecon.Core;
using FullRecon.Syntax;
using FullRecon.Syntax.Terms;
using Xunit;

namespace FullRecon.Tests;

public class EvaluationTests
{
    private static Context EmptyCtx => new Context();

    [Fact(DisplayName = "Eval returns true as value")]
    public void EvalTrueIsValue()
    {
        var info = new UnknownInfo();
        var term = new True(info);
        var result = Evaluation.Eval(EmptyCtx, term);
        Assert.IsType<True>(result);
    }

    [Fact(DisplayName = "Eval reduces if-true")]
    public void EvalIfTrue()
    {
        var info = new UnknownInfo();
        var term = new If(info, new True(info), new False(info), new True(info));
        var result = Evaluation.Eval(EmptyCtx, term);
        Assert.IsType<False>(result);
    }

    [Fact(DisplayName = "Eval reduces if-false")]
    public void EvalIfFalse()
    {
        var info = new UnknownInfo();
        var term = new If(info, new False(info), new True(info), new False(info));
        var result = Evaluation.Eval(EmptyCtx, term);
        Assert.IsType<False>(result);
    }

    [Fact(DisplayName = "Eval reduces application")]
    public void EvalApplication()
    {
        var info = new UnknownInfo();
        // (lambda x:Bool. x) true -> true
        var abs = new Abs(info, "x", new TypeBool(), new Var(info, 0, 1));
        var app = new App(info, abs, new True(info));
        var result = Evaluation.Eval(EmptyCtx, app);
        Assert.IsType<True>(result);
    }

    [Fact(DisplayName = "Eval reduces let")]
    public void EvalLet()
    {
        var info = new UnknownInfo();
        // let x = true in x -> true
        var let = new Let(info, "x", new True(info), new Var(info, 0, 1));
        var result = Evaluation.Eval(EmptyCtx, let);
        Assert.IsType<True>(result);
    }

    [Fact(DisplayName = "Eval reduces succ 0 to 1")]
    public void EvalSuccZero()
    {
        var info = new UnknownInfo();
        var term = new Succ(info, new Zero(info));
        var result = Evaluation.Eval(EmptyCtx, term);
        var succ = Assert.IsType<Succ>(result);
        Assert.IsType<Zero>(succ.Of);
    }

    [Fact(DisplayName = "Eval reduces pred (succ 0) to 0")]
    public void EvalPredSuccZero()
    {
        var info = new UnknownInfo();
        var term = new Pred(info, new Succ(info, new Zero(info)));
        var result = Evaluation.Eval(EmptyCtx, term);
        Assert.IsType<Zero>(result);
    }

    [Fact(DisplayName = "Eval reduces iszero 0 to true")]
    public void EvalIsZeroZero()
    {
        var info = new UnknownInfo();
        var term = new IsZero(info, new Zero(info));
        var result = Evaluation.Eval(EmptyCtx, term);
        Assert.IsType<True>(result);
    }
}
