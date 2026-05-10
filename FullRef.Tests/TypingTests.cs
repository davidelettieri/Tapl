using Common;
using FullRef.Core;
using FullRef.Syntax;
using FullRef.Syntax.Terms;
using Xunit;

namespace FullRef.Tests;

public class TypingTests
{
    [Fact]
    public void TypeOfRefAndDerefTerms()
    {
        var info = new UnknownInfo();
        var context = new Context();
        var reference = new Ref(info, new True(info));

        var refType = Assert.IsType<TypeRef>(Typing.TypeOf(context, reference));
        Assert.IsType<TypeBool>(refType.Type);

        var deref = new Deref(info, reference);
        Assert.IsType<TypeBool>(Typing.TypeOf(context, deref));
    }

    [Fact]
    public void TypeOfAssignmentIsUnit()
    {
        var info = new UnknownInfo();
        var context = new Context().AddBinding("r", new FullRef.Syntax.Bindings.VarBind(new TypeRef(new TypeNat())));
        var assign = new Assign(info, new Var(info, 0, 1), new Succ(info, new Zero(info)));

        Assert.IsType<TypeUnit>(Typing.TypeOf(context, assign));
    }
}
