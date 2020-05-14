using Arith.Terms;
using static Arith.Functions;
using Xunit;

namespace Arith.Tests
{
    public class ParseEvalTests
    {
        [Fact(DisplayName = "Parse & eval with ()")]
        public void ParseWithParenthesis()
        {
            // Arrange 
            var s = "if (iszero succ 0) then (succ 0) else (pred false)";

            // Act
            var result = Eval(Parse(s));

            // Assert
            Assert.IsType<Pred>(result);
            var pred = result as Pred;
            Assert.IsType<False>(pred?.Of);
        }
    }
}
