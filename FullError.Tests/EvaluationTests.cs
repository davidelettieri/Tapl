using Common;
using FullError.Syntax;
using FullError.Syntax.Terms;
using FullError.Core;
using Xunit;

namespace FullError.Tests;

public class EvaluationTests
{
    private static Context EmptyCtx() => new Context();

    [Fact(DisplayName = "Eval true returns true")]
    public void EvalTrueReturnsTrue()
    {
        var t = new True(new UnknownInfo());
        var result = Evaluation.Eval(EmptyCtx(), t);
        Assert.IsType<True>(result);
    }

    [Fact(DisplayName = "Eval if true then false else true evaluates to false")]
    public void EvalIfTrueThenFalseElseTrue()
    {
        var info = new UnknownInfo();
        var t = new If(info, new True(info), new False(info), new True(info));
        var result = Evaluation.Eval(EmptyCtx(), t);
        Assert.IsType<False>(result);
    }

    [Fact(DisplayName = "Eval if error evaluates to error")]
    public void EvalIfErrorReturnsError()
    {
        var info = new UnknownInfo();
        var t = new If(info, new Error(info), new True(info), new False(info));
        var result = Evaluation.Eval(EmptyCtx(), t);
        Assert.IsType<Error>(result);
    }

    [Fact(DisplayName = "Eval (lambda x:Bool.x) true returns true")]
    public void EvalApplicationReturnsTrueFromBody()
    {
        var info = new UnknownInfo();
        var abs = new Abs(info, "x", new TypeBool(), new Var(info, 0, 1));
        var app = new App(info, abs, new True(info));
        var result = Evaluation.Eval(EmptyCtx(), app);
        Assert.IsType<True>(result);
    }

    [Fact(DisplayName = "Eval error propagates through application")]
    public void EvalErrorPropagatesThroughApplication()
    {
        // error true -> error
        var info = new UnknownInfo();
        var app = new App(info, new Error(info), new True(info));
        var result = Evaluation.Eval(EmptyCtx(), app);
        Assert.IsType<Error>(result);
    }

    [Fact(DisplayName = "Eval value applied to error propagates error")]
    public void EvalValueAppliedToErrorPropagatesError()
    {
        // (lambda x:Bool.x) error -> error
        var info = new UnknownInfo();
        var abs = new Abs(info, "x", new TypeBool(), new Var(info, 0, 1));
        var err = new Error(info);
        var app = new App(info, abs, err);
        var result = Evaluation.Eval(EmptyCtx(), app);
        Assert.IsType<Error>(result);
    }
}
