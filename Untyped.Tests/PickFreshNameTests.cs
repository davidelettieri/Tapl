using Common;
using Xunit;

namespace Untyped.Tests;

public class PickFreshNameTests
{
    [Fact(DisplayName = "Pick fresh from empty context")]
    public void PickFreshFromEmpty()
    {
        // Arrange
        var v = "x";
        var ctx = new Context();

        // Act
        var (ctx1, f) = ctx.PickFreshName(v);

        // Assert
        Assert.Equal(v, f);
        Assert.Equal(1, ctx1.Length);
    }

    [Fact(DisplayName = "Pick fresh from not empty context")]
    public void PickFreshFromNotEmpty()
    {
        // Arrange
        var v = "y";
        var ctx = new Context();
        ctx = ctx.AddName("x");

        // Act
        var (ctx1, f) = ctx.PickFreshName(v);

        // Assert
        Assert.Equal(v, f);
        Assert.Equal(2, ctx1.Length);
    }

    [Fact(DisplayName = "Pick fresh from not empty context with colliding name")]
    public void PickFreshFromNotEmptyWithCollidingName()
    {
        // Arrange
        var v = "x";
        var ctx = new Context();
        ctx = ctx.AddName("x");

        // Act
        var (ctx1, f) = ctx.PickFreshName(v);

        // Assert
        Assert.Equal("x'", f);
        Assert.Equal(2, ctx1.Length);
    }
}