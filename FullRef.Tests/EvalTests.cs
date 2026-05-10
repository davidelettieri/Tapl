using Common;
using FullRef.Core;
using FullRef.Syntax.Terms;
using Xunit;

namespace FullRef.Tests;

public class EvalTests
{
    [Fact]
    public void EvalPropagatesStoreAcrossAssignmentAndDereference()
    {
        var info = new UnknownInfo();
        var zero = new Zero(info);
        var one = new Succ(info, zero);
        var term = new Let(
            info,
            "r",
            new Ref(info, zero),
            new Let(
                info,
                "_",
                new Assign(info, new Var(info, 0, 1), one),
                new Deref(info, new Var(info, 1, 2))));

        var (result, store) = Evaluation.Eval(new Context(), Store.Empty, term);

        var succ = Assert.IsType<Succ>(result);
        Assert.IsType<Zero>(succ.Of);
        Assert.Single(store.Terms);
        var stored = Assert.IsType<Succ>(store.Terms[0]);
        Assert.IsType<Zero>(stored.Of);
    }
}
