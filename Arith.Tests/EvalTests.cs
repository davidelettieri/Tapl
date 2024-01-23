using Arith.Terms;
using static Arith.Functions;
using Xunit;

namespace Arith.Tests;

public class EvalTests
{
    [Fact(DisplayName = "Eval 'true'")]
    public void EvalTrue()
    {
        // Arrange 
        var s = new True();

        // Act
        var term = Eval(s);

        // Assert
        Assert.IsType<True>(term);
    }

    [Fact(DisplayName = "Eval 'false'")]
    public void EvalFalse()
    {
        // Arrange 
        var s = new False();

        // Act
        var term = Eval(s);

        // Assert
        Assert.IsType<False>(term);
    }

    [Fact(DisplayName = "Eval 'succ false'")]
    public void EvalSuccFalse()
    {
        // Arrange 
        var s = new Succ(new False());

        // Act
        var term = Eval(s);

        // Assert
        var succ = Assert.IsType<Succ>(term);
        Assert.IsType<False>(succ.Of);
    }

    [Fact(DisplayName = "Eval 'pred 0'")]
    public void EvalPredZero()
    {
        // Arrange 
        var s = new Pred(new Zero());

        // Act
        var term = Eval(s);

        // Assert
        Assert.IsType<Zero>(term);
    }

    [Fact(DisplayName = "Eval 'iszero 0'")]
    public void EvalIsZeroZero()
    {
        // Arrange 
        var s = new IsZero(new Zero());

        // Act
        var term = Eval(s);

        // Assert
        Assert.IsType<True>(term);
    }

    [Fact(DisplayName = "Eval 'if true then 0 else false'")]
    public void EvalIfTrue()
    {
        // Arrange 
        var s = new If(new True(), new Zero(), new False());

        // Act
        var term = Eval(s);

        // Assert
        Assert.IsType<Zero>(term);
    }

    [Fact(DisplayName = "Eval 'if false then 0 else false'")]
    public void EvalIfFalse()
    {
        // Arrange 
        var s = new If(new False(), new Zero(), new False());

        // Act
        var term = Eval(s);

        // Assert
        Assert.IsType<False>(term);
    }

    [Fact(DisplayName = "Eval 'pred succ 0'")]
    public void PredSuccZero()
    {
        // Arrange 
        var s = new Pred(new Succ(new Zero()));

        // Act
        var term = Eval(s);

        // Assert
        Assert.IsType<Zero>(term);
    }

    [Fact(DisplayName = "Eval 'iszero 0'")]
    public void IsZeroZero()
    {
        // Arrange 
        var s = new IsZero(new Zero());

        // Act
        var term = Eval(s);

        // Assert
        Assert.IsType<True>(term);
    }

    [Fact(DisplayName = "Eval 'iszero succ 0'")]
    public void IsZeroSuccZero()
    {
        // Arrange 
        var s = new IsZero(new Succ(new Zero()));

        // Act
        var term = Eval(s);

        // Assert
        Assert.IsType<False>(term);
    }

    [Fact(DisplayName = "Eval 'iszero false'")]
    public void IsZeroFalse()
    {
        // Arrange 
        var s = new IsZero(new Succ(new Zero()));

        // Act
        var term = Eval(s);

        // Assert
        Assert.IsType<False>(term);
    }
}