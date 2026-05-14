using Common;
using FullUntyped.Core;
using FullUntyped.Syntax;
using FullUntyped.Syntax.Terms;
using Xunit;

namespace FullUntyped.Tests;

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
}
