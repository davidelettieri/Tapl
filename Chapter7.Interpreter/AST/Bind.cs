namespace Chapter7.Interpreter.AST
{
    public class Bind : IExpression
    {
        public string Variable { get; }
        public Bind(string variable)
        {
            Variable = variable;
        }
    }
}
