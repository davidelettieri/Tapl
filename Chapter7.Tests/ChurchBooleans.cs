using Chapter7.Terms;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using static Chapter7.Functions;

namespace Chapter7.Tests
{
    public class ChurchBooleans
    {
        private const string TrueString = "\\t.\\f.t";
        private const string FalseString = "\\t.\\f.f";
        private const string IfString = "\\l.\\m.\\n.lmn";

        [Fact(DisplayName = "Parse True lambda")]
        public void ParseTrueLambda()
        {
            // Act
            var t = Parse(TrueString);

            // Assert
            Assert.IsType<Abs>(t);
            var abs = t as Abs;
            Assert.IsType<Abs>(abs?.Body);
            Assert.Equal("t", abs?.BoundedVariable);
            var absi = abs.Body as Abs;
            Assert.Equal("f", absi?.BoundedVariable);
            Assert.IsType<Var>(absi?.Body);
        }

        [Fact(DisplayName = "Parse False lambda")]
        public void ParseFalseLambda()
        {
            // Act
            var t = Parse(FalseString);

            // Assert
            Assert.IsType<Abs>(t);
            var abs = t as Abs;
            Assert.IsType<Abs>(abs?.Body);
            Assert.Equal("t", abs?.BoundedVariable);
            var absi = abs.Body as Abs;
            Assert.Equal("f", absi?.BoundedVariable);
            Assert.IsType<Var>(absi?.Body);
        }
    }
}
