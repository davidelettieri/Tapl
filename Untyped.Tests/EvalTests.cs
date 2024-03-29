﻿using Untyped.Terms;
using Xunit;
using static Untyped.Functions;
using Common;

namespace Untyped.Tests;

public class EvalTests
{
    private static readonly Context EmptyContext = new Context();

    [Fact(DisplayName = "Eval variable with empty context")]
    public void EvalVariable()
    {
        // Arrange
        var v = new Var(0, 0);

        // Act
        var t = Eval(EmptyContext, v);

        // Assert
        Assert.IsType<Var>(t);
    }

    [Fact(DisplayName = "Eval lambda abstraction with empty context")]
    public void EvalLambdaAbstraction()
    {
        // Arrange
        var v = new Abs(new Var(0, 0), "x");

        // Act
        var t = Eval(EmptyContext, v);

        // Assert
        Assert.IsType<Abs>(t);
    }

    [Fact(DisplayName = "Eval (\\x.x) (\\x.xx)")]
    public void EvalApplication()
    {
        // Arrange
        var left = new Abs(new Var(0, 0), "x");
        var right = new Abs(new App(new Var(0, 0), new Var(0, 0)), "x");
        var v = new App(left, right);
        // Act
        var t = Eval(EmptyContext, v);

        // Assert
        var abs = Assert.IsType<Abs>(t);
        Assert.IsType<App>(abs.Body);
    }
}