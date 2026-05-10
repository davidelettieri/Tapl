using Common;
using FullRef.Syntax;
using FullRef.Syntax.Terms;
using Xunit;

namespace FullRef.Tests;

public class ParseTests
{
    [Fact]
    public void ParseLambdaWithRefTypeAndDerefBody()
    {
        var (commands, _) = FullRef.Functions.Parse("lambda r: Ref Nat. !r;")(new Context());

        var eval = Assert.IsType<Eval>(commands.Peek());
        var abs = Assert.IsType<Abs>(eval.Term);
        var refType = Assert.IsType<TypeRef>(abs.Type);
        Assert.IsType<TypeNat>(refType.Type);
        var deref = Assert.IsType<Deref>(abs.Body);
        var variable = Assert.IsType<Var>(deref.Term);
        Assert.Equal(0, variable.Index);
        Assert.Equal(1, variable.ContextLength);
    }

    [Fact]
    public void ParseAssignmentInLetBody()
    {
        var (commands, _) = FullRef.Functions.Parse("let r = ref 0 in r := succ 0;")(new Context());

        var eval = Assert.IsType<Eval>(commands.Peek());
        var letTerm = Assert.IsType<Let>(eval.Term);
        Assert.IsType<Ref>(letTerm.LetTerm);
        var assign = Assert.IsType<Assign>(letTerm.InTerm);
        var variable = Assert.IsType<Var>(assign.Left);
        Assert.Equal(0, variable.Index);
        Assert.Equal(1, variable.ContextLength);
        Assert.IsType<Succ>(assign.Right);
    }
}
