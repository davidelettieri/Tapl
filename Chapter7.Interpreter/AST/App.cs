namespace Chapter7.Interpreter.AST
{
    /// <summary>
    /// Application term (xy)
    /// </summary>
    public class App : IExpression
    {
        public IExpression Left { get; }
        public IExpression Right { get; }
        /// <summary>
        /// Application term (xy)
        /// </summary>
        /// <param name="left">The first term in the application</param>
        /// <param name="right">The second term in the application</param>
        public App(IExpression left, IExpression right)
        {
            Left = left;
            Right = right;
        }
    }
}
