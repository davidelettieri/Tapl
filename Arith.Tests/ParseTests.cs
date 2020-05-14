using Arith.Terms;
using System;
using Xunit;
using static Arith.Functions;

namespace Arith.Tests
{
    public class ParseTests
    {
        [Fact(DisplayName = "Parse 'true'")]
        public void ParseTrue()
        {
            // Arrange 
            var s = "true";

            // Act
            var term = Parse(s);

            // Assert
            Assert.IsType<True>(term);
        }

        [Fact(DisplayName = "Parse 'false'")]
        public void ParseFalse()
        {
            // Arrange 
            var s = "false";

            // Act
            var term = Parse(s);

            // Assert
            Assert.IsType<False>(term);
        }

        [Fact(DisplayName = "Parse '0'")]
        public void ParseZero()
        {
            // Arrange 
            var s = "0";

            // Act
            var term = Parse(s);

            // Assert
            Assert.IsType<Zero>(term);
        }

        [Fact(DisplayName = "Parse 'succ 0'")]
        public void ParseSuccZero()
        {
            // Arrange 
            var s = "succ 0";

            // Act
            var term = Parse(s);

            // Assert
            Assert.IsType<Succ>(term);
            var succ = term as Succ;
            Assert.IsType<Zero>(succ?.Of);
        }

        [Fact(DisplayName = "Parse 'pred 0'")]
        public void ParsePrevZero()
        {
            // Arrange 
            var s = "pred 0";

            // Act
            var term = Parse(s);

            // Assert
            Assert.IsType<Pred>(term);
            var pred = term as Pred;
            Assert.IsType<Zero>(pred?.Of);
        }

        [Fact(DisplayName = "Parse 'iszero false'")]
        public void ParseIsZeroFalse()
        {
            // Arrange 
            var s = "iszero false";

            // Act
            var term = Parse(s);

            // Assert
            Assert.IsType<IsZero>(term);
            var succ = term as IsZero;
            Assert.IsType<False>(succ?.Term);
        }

        [Fact(DisplayName = "Parse 'if'")]
        public void ParseIf()
        {
            // Arrange 
            var s = "if false then 0 else false";

            // Act
            var term = Parse(s);

            // Assert
            Assert.IsType<If>(term);
            var _if = term as If;
            Assert.IsType<False>(_if?.Condition);
            Assert.IsType<Zero>(_if?.Then);
            Assert.IsType<False>(_if?.Else);
        }

        [Fact(DisplayName = "Parse with ()")]
        public void ParseWithParenthesis()
        {
            // Arrange 
            var s = "if (iszero succ 0) then (succ 0) else (pred false)";

            // Act
            var term = Parse(s);

            // Assert
            Assert.IsType<If>(term);
            var _if = term as If;
            Assert.IsType<IsZero>(_if?.Condition);
            Assert.IsType<Succ>(_if?.Then);
            Assert.IsType<Pred>(_if?.Else);
        }
    }
}
