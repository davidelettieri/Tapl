using Chapter7.Terms;
using Xunit;
using static Chapter7.Functions;

namespace Untyped.Tests
{
    public class ChurchBooleans
    {
        //private const string TrueString = "\\t.\\f.t";
        //private const string FalseString = "\\t.\\f.f";
        //private const string IfString = "\\l.\\m.\\n.lmn";

        //[Fact(DisplayName = "Parse True lambda")]
        //public void ParseTrueLambda()
        //{
        //    // Act
        //    var t = Parse(TrueString);
        //    var v = t(new Context());
        //    // Assert
        //    Assert.IsType<Abs>(v);
        //    var abs = v as Abs;
        //    Assert.IsType<Abs>(abs?.Body);
        //    Assert.Equal("t", abs?.BoundedVariable);
        //    var absi = abs.Body as Abs;
        //    Assert.Equal("f", absi?.BoundedVariable);
        //    Assert.IsType<Var>(absi?.Body);
        //    var b = absi?.Body as Var;
        //    Assert.Equal(1, b.Index);
        //}

        //[Fact(DisplayName = "Parse False lambda")]
        //public void ParseFalseLambda()
        //{
        //    // Act
        //    var t = Parse(FalseString);
        //    var v = t(new Context());
        //    // Assert
        //    Assert.IsType<Abs>(v);
        //    var abs = v as Abs;
        //    Assert.IsType<Abs>(abs?.Body);
        //    Assert.Equal("t", abs?.BoundedVariable);
        //    var absi = abs.Body as Abs;
        //    Assert.Equal("f", absi?.BoundedVariable);
        //    Assert.IsType<Var>(absi?.Body);
        //    var b = absi?.Body as Var;
        //    Assert.Equal(0, b.Index);
        //}

        //[Fact(DisplayName = "Parse If Lambda")]
        //public void ParseIfLambda()
        //{
        //    // Act
        //    var t = Parse(IfString);
        //    var v = t(new Context());

        //    // Assert
        //    Assert.IsType<Abs>(v);
        //    var abs = v as Abs;
        //    Assert.IsType<Abs>(abs?.Body);
        //    Assert.Equal("l", abs?.BoundedVariable);
        //    var absi = abs.Body as Abs;
        //    Assert.Equal("m", absi?.BoundedVariable);
        //    Assert.IsType<Abs>(absi?.Body);
        //    var absii = absi.Body as Abs;
        //    Assert.Equal("n", absii?.BoundedVariable);
        //    Assert.IsType<App>(absii?.Body);
        //}

        //[Fact(DisplayName = "Evaluate If Lambda")]
        //public void EvaluateIfLambda()
        //{
        //    // Act
        //    var t = Parse($"({IfString}) ({TrueString}) (\\x.x) (\\y.yy)");
        //    var emptyCtx = new Context();
        //    var v = Eval(emptyCtx, t(emptyCtx));

        //    // Assert
        //    Assert.IsType<Abs>(v);
        //    var abs = v as Abs;
        //}
    }
}
