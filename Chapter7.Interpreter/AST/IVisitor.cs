namespace Chapter7.Interpreter.AST
{
    interface Visitor<T>
    {
        T Visit(Abs expr);
        T Visit(App expr);
        T Visit(Var expr);
        T Visit(Bind expr);
    }
}
