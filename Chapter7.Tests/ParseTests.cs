using Chapter7.Terms;
using System;
using Xunit;
using static Chapter7.Functions;
namespace Chapter7.Tests
{
    public class ParseTests
    {
        [Fact(DisplayName = "Parse x")]
        public void ParseVariable()
        {
            // Arrange 
            var s = "x";

            // Act
            var t = Parse(s);

            // Assert
            Assert.IsType<Var>(t);
        }

        [Fact(DisplayName = "Parse xy")]
        public void ParseApplication()
        {
            // Arrange 
            var s = "xy";

            // Act
            var t = Parse(s);

            // Assert
            Assert.IsType<App>(t);
            var app = t as App;
            Assert.IsType<Var>(app?.Left);
            Assert.IsType<Var>(app?.Right);
        }

        [Fact(DisplayName = "Parse \\x.xx")]
        public void ParseLambdaAbstraction()
        {
            // Arrange 
            var s = "\\x.xx";

            // Act
            var t = Parse(s);

            // Assert
            Assert.IsType<Abs>(t);
            var abs = t as Abs;
            Assert.Equal("x", abs.BoundedVariable);
            Assert.IsType<App>(abs?.Body);
            var app = abs?.Body as App;
            Assert.IsType<Var>(app?.Left);
            Assert.IsType<Var>(app?.Right);
        }
        //lambda x.x;
        //;

        [Fact(DisplayName = "Parse \\x.x")]
        public void ParseLambdaId()
        {
            // Arrange 
            var s = "\\x.x";

            // Act
            var t = Parse(s);

            // Assert
            Assert.IsType<Abs>(t);
            var abs = t as Abs;
            Assert.Equal("x", abs.BoundedVariable);
            Assert.IsType<Var>(abs?.Body);
        }

        [Fact(DisplayName = "Parse (\\x.x) (\\x.xx)")]
        public void ParseAppLambdaLambda()
        {
            // Arrange 
            var s = "(\\x.x) (\\x.xx)";

            // Act
            var t = Parse(s);

            // Assert
            Assert.IsType<App>(t);
            var app = t as App;
            var left = app?.Left;
            var right = app?.Right;
            Assert.IsType<Abs>(left);
            Assert.IsType<Abs>(right);
            var absl = left as Abs;
            var absr = right as Abs;
            Assert.IsType<Var>(absl.Body);
            Assert.Equal("x", absl.BoundedVariable);
            Assert.IsType<App>(absr.Body);
        }
    }
}
