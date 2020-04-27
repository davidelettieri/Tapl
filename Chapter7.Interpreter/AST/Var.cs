namespace Chapter7.Interpreter.AST
{
    public class Var : IExpression
    {
        public int Index { get; }
        public int ContextLength { get; }

        /// <summary>
        /// Variable term
        /// </summary>
        /// <param name="index">De bruijn index</param>
        /// <param name="ctxl">Context length</param>
        public Var(int index, int ctxl)
        {
            Index = index;
            ContextLength = ctxl;
        }
    }
}
