using Arith.Terms;
using Xunit;
using static Arith.Functions;

namespace Arith.Tests;

public class ParseTests
{
    [Fact(DisplayName = "Parse 'true'")]
    public void ParseTrue()
    {
        // Arrange 
        var s = "true";

        // Act
        var term = Parse(s);

        // Assert
        Assert.IsType<True>(term);
    }

    [Fact(DisplayName = "Parse 'false'")]
    public void ParseFalse()
    {
        // Arrange 
        var s = "false";

        // Act
        var term = Parse(s);

        // Assert
        Assert.IsType<False>(term);
    }

    [Fact(DisplayName = "Parse '0'")]
    public void ParseZero()
    {
        // Arrange 
        var s = "0";

        // Act
        var term = Parse(s);

        // Assert
        Assert.IsType<Zero>(term);
    }

    [Fact(DisplayName = "Parse 'succ 0'")]
    public void ParseSuccZero()
    {
        // Arrange 
        var s = "succ 0";

        // Act
        var term = Parse(s);

        // Assert
        var succ = Assert.IsType<Succ>(term);
        Assert.IsType<Zero>(succ.Of);
    }

    [Fact(DisplayName = "Parse 'pred 0'")]
    public void ParsePrevZero()
    {
        // Arrange 
        var s = "pred 0";

        // Act
        var term = Parse(s);

        // Assert
        var pred = Assert.IsType<Pred>(term);
        Assert.IsType<Zero>(pred.Of);
    }

    [Fact(DisplayName = "Parse 'iszero false'")]
    public void ParseIsZeroFalse()
    {
        // Arrange 
        var s = "iszero false";

        // Act
        var term = Parse(s);

        // Assert
        var succ = Assert.IsType<IsZero>(term);
        Assert.IsType<False>(succ.Term);
    }

    [Fact(DisplayName = "Parse 'if'")]
    public void ParseIf()
    {
        // Arrange 
        var s = "if false then 0 else false";

        // Act
        var term = Parse(s);

        // Assert
        var @if = Assert.IsType<If>(term);
        Assert.IsType<False>(@if.Condition);
        Assert.IsType<Zero>(@if.Then);
        Assert.IsType<False>(@if.Else);
    }

    [Fact(DisplayName = "Parse with ()")]
    public void ParseWithParenthesis()
    {
        // Arrange 
        var s = "if (iszero succ 0) then (succ 0) else (pred false)";

        // Act
        var term = Parse(s);

        // Assert
        var @if = Assert.IsType<If>(term);
        Assert.IsType<IsZero>(@if.Condition);
        Assert.IsType<Succ>(@if.Then);
        Assert.IsType<Pred>(@if.Else);
    }
}