using Common;
using FullPoly.Core;
using FullPoly.Syntax;
using FullPoly.Syntax.Terms;
using Xunit;

namespace FullPoly.Tests;

public class PolyCoreTests
{
    [Fact]
    public void TypeApplicationEvaluatesAndTypes()
    {
        var info = new UnknownInfo();
        var ctx = new Context();
        var term = new TApp(
            info,
            new TAbs(info, "X", new Abs(info, "x", new TypeVar(0, 1), new Var(info, 0, 2))),
            new TypeBool());

        var eval = Evaluation.Eval(ctx, term);
        var abs = Assert.IsType<Abs>(eval);
        Assert.IsType<TypeBool>(abs.Type);
        Assert.IsType<Var>(abs.Body);

        var ty = Typing.TypeOf(ctx, term);
        var tyArr = Assert.IsType<TypeArrow>(ty);
        Assert.IsType<TypeBool>(tyArr.From);
        Assert.IsType<TypeBool>(tyArr.To);
    }

    [Fact]
    public void PackUnpackEvaluatesAndTypes()
    {
        var info = new UnknownInfo();
        var ctx = new Context();
        var pack = new Pack(info, new TypeNat(), new Zero(info), new TypeSome("Y", new TypeVar(0, 1)));
        var unpack = new Unpack(info, "X", "x", pack, new Var(info, 0, 2));

        var eval = Evaluation.Eval(ctx, unpack);
        Assert.IsType<Zero>(eval);

        var ty = Typing.TypeOf(ctx, unpack);
        Assert.IsType<TypeVar>(ty);
    }
}
