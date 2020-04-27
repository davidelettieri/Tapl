namespace Chapter7.Interpreter.AST
{
    /// <summary>
    /// Lambda abstraction term λx.y
    /// </summary>
    public class Abs : IExpression
    {
        public IExpression Body { get; }
        public string BoundedVariable { get; }

        /// <summary>
        /// Lambda abstraction term λx.y
        /// </summary>
        /// <param name="body">The body of the lambda abstraction</param>
        /// <param name="bv">The bounded variable</param>
        public Abs(IExpression body, string bv)
        {
            Body = body;
            BoundedVariable = bv;
        }
    }
}
