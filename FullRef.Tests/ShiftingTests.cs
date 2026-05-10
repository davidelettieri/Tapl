using Common;
using FullRef.Core;
using FullRef.Syntax;
using FullRef.Syntax.Terms;
using Xunit;

namespace FullRef.Tests;

public class ShiftingTests
{
    [Fact(DisplayName = "TermShift traverses Ascribe term")]
    public void TermShiftTraversesAscribeTerm()
    {
        var info = new UnknownInfo();
        var term = new Ascribe(info, new Var(info, 0, 1), new TypeBool());

        var shifted = Shifting.TermShift(1, term);

        var ascribe = Assert.IsType<Ascribe>(shifted);
        var variable = Assert.IsType<Var>(ascribe.Term);
        Assert.Equal(1, variable.Index);
        Assert.Equal(2, variable.ContextLength);
        Assert.IsType<TypeBool>(ascribe.Type);
    }

    [Fact(DisplayName = "TermShift traverses ref and assign terms without shifting locations")]
    public void TermShiftTraversesReferenceTerms()
    {
        var info = new UnknownInfo();
        var term = new Assign(
            info,
            new Loc(info, 3),
            new Ref(info, new Deref(info, new Var(info, 0, 1))));

        var shifted = Assert.IsType<Assign>(Shifting.TermShift(1, term));
        var loc = Assert.IsType<Loc>(shifted.Left);
        Assert.Equal(3, loc.Location);
        var reference = Assert.IsType<Ref>(shifted.Right);
        var deref = Assert.IsType<Deref>(reference.Term);
        var variable = Assert.IsType<Var>(deref.Term);
        Assert.Equal(1, variable.Index);
        Assert.Equal(2, variable.ContextLength);
    }
}
