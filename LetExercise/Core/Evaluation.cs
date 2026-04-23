using Common;
using LetExercise.Syntax;
using static LetExercise.Core.Substitution;

namespace LetExercise.Core;

public static class Evaluation
{
    private static bool IsVal(Context _, ITerm t) =>
        t switch
        {
            True => true,
            False => true,
            Abs => true,
            _ => false
        };

    private static ITerm Eval1(Context ctx, ITerm t) =>
        t switch
        {
            Let let when IsVal(ctx, let.LetTerm) => TermSubsTop(let.LetTerm, let.InTerm),
            Let let => new Let(let.Info, let.Variable, Eval1(ctx, let.LetTerm), let.InTerm),
            App { Left: Abs abs } app when IsVal(ctx, app.Right)
                => TermSubsTop(app.Right, abs.Body),
            App app when IsVal(ctx, app.Left) => new App(app.Info, app.Left, Eval1(ctx, app.Right)),
            App app => new App(app.Info, Eval1(ctx, app.Left), app.Right),
            If { Condition: True } ift => ift.Then,
            If { Condition: False } ift => ift.Else,
            If ift => new If(ift.Info, Eval1(ctx, ift.Condition), ift.Then, ift.Else),
            _ => throw new NoRulesAppliesException()
        };

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
}