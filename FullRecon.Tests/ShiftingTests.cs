using Common;
using FullRecon.Core;
using FullRecon.Syntax;
using FullRecon.Syntax.Terms;
using Xunit;

namespace FullRecon.Tests;

public class ShiftingTests
{
    [Fact(DisplayName = "TermShift traverses Abs term with typed parameter")]
    public void TermShiftTraversesAbsTypedTerm()
    {
        var info = new UnknownInfo();
        var term = new Abs(info, "x", new TypeBool(), new Var(info, 0, 1));

        var shifted = Shifting.TermShift(1, term);

        var abs = Assert.IsType<Abs>(shifted);
        Assert.IsType<TypeBool>(abs.Type);
        var body = Assert.IsType<Var>(abs.Body);
        Assert.Equal(0, body.Index);
        Assert.Equal(2, body.ContextLength);
    }

    [Fact(DisplayName = "TermShift traverses Abs term with untyped parameter")]
    public void TermShiftTraversesAbsUntypedTerm()
    {
        var info = new UnknownInfo();
        var term = new Abs(info, "x", null, new Var(info, 0, 1));

        var shifted = Shifting.TermShift(1, term);

        var abs = Assert.IsType<Abs>(shifted);
        Assert.Null(abs.Type);
        var body = Assert.IsType<Var>(abs.Body);
        Assert.Equal(0, body.Index);
        Assert.Equal(2, body.ContextLength);
    }

    [Fact(DisplayName = "TermShift traverses Let term")]
    public void TermShiftTraversesLetTerm()
    {
        var info = new UnknownInfo();
        var term = new Let(info, "x", new Var(info, 0, 1), new Var(info, 0, 2));

        var shifted = Shifting.TermShift(1, term);

        var let = Assert.IsType<Let>(shifted);
        var letVar = Assert.IsType<Var>(let.LetTerm);
        Assert.Equal(1, letVar.Index);
        Assert.Equal(2, letVar.ContextLength);
        var inVar = Assert.IsType<Var>(let.InTerm);
        Assert.Equal(0, inVar.Index);
        Assert.Equal(3, inVar.ContextLength);
    }

    [Fact(DisplayName = "TermShift traverses Succ term")]
    public void TermShiftTraversesSuccTerm()
    {
        var info = new UnknownInfo();
        var term = new Succ(info, new Var(info, 0, 1));

        var shifted = Shifting.TermShift(1, term);

        var succ = Assert.IsType<Succ>(shifted);
        var inner = Assert.IsType<Var>(succ.Of);
        Assert.Equal(1, inner.Index);
        Assert.Equal(2, inner.ContextLength);
    }
}
