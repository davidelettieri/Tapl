using Common;
using FullError.Core;
using FullError.Syntax;
using FullError.Syntax.Terms;
using Xunit;

namespace FullError.Tests;

public class ShiftingTests
{
    [Fact(DisplayName = "TermShift traverses Abs term")]
    public void TermShiftTraversesAbsTerm()
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

    [Fact(DisplayName = "TermShift traverses Try term")]
    public void TermShiftTraversesTryTerm()
    {
        var info = new UnknownInfo();
        var term = new Try(info, new Var(info, 0, 1), new Var(info, 1, 2));

        var shifted = Shifting.TermShift(1, term);

        var tr = Assert.IsType<Try>(shifted);
        var t1 = Assert.IsType<Var>(tr.Term);
        Assert.Equal(1, t1.Index);
        Assert.Equal(2, t1.ContextLength);
        var t2 = Assert.IsType<Var>(tr.Handler);
        Assert.Equal(2, t2.Index);
        Assert.Equal(3, t2.ContextLength);
    }

    [Fact(DisplayName = "TermShift traverses Error term (no shift of index)")]
    public void TermShiftTraversesErrorTerm()
    {
        var info = new UnknownInfo();
        var term = new Error(info);

        var shifted = Shifting.TermShift(1, term);

        Assert.IsType<Error>(shifted);
    }
}
