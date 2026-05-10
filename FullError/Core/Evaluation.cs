using Common;
using FullError.Syntax;
using FullError.Syntax.Bindings;
using FullError.Syntax.Terms;
using static FullError.Core.Substitution;

namespace FullError.Core;

public static class Evaluation
{
    private static bool IsVal(Context ctx, ITerm t) => t switch
    {
        True => true,
        False => true,
        Abs => true,
        _ => false
    };

    private static ITerm Eval1(Context ctx, ITerm t) => t switch
    {
        If { Condition: True } ift => ift.Then,
        If { Condition: False } ift => ift.Else,
        If { Condition: Error err } => new Error(err.Info),
        If ift => new If(ift.Info, Eval1(ctx, ift.Condition), ift.Then, ift.Else),
        Var var => VarLookup(ctx, var),
        App { Left: Error err } => new Error(err.Info),
        App app when IsVal(ctx, app.Left) && app.Right is Error err => new Error(err.Info),
        App { Left: Abs abs } app when IsVal(ctx, app.Right) => TermSubstTop(app.Right, abs.Body),
        App app when IsVal(ctx, app.Left) => new App(app.Info, app.Left, Eval1(ctx, app.Right)),
        App app => new App(app.Info, Eval1(ctx, app.Left), app.Right),
        _ => throw new NoRulesAppliesException()
    };

    private static ITerm VarLookup(Context ctx, Var var)
    {
        var b = ctx.GetBinding(var.Index);

        if (b is TermAbbBind tab)
            return tab.Term;

        throw new NoRulesAppliesException();
    }

    public static ITerm Eval(Context ctx, ITerm t)
    {
        try
        {
            var t1 = Eval1(ctx, t);
            return Eval(ctx, t1);
        }
        catch (NoRulesAppliesException)
        {
            return t;
        }
    }

    public static IBinding EvalBinding(Context ctx, IBinding bind) => bind switch
    {
        TermAbbBind tab => new TermAbbBind(Eval(ctx, tab.Term), tab.Type),
        _ => bind
    };
}
