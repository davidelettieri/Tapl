using Common;
using Untyped.Terms;
using static Untyped.Core.Substitution;

namespace Untyped.Core;

public static class Evaluation
{
    private static bool IsVal(ITerm t) => t is Abs;

    private static ITerm Eval1(Context ctx, ITerm t)
    {
        return t switch
        {
            App { Left: Abs abs } app when IsVal(app.Right) => TermSubsTop(app.Right, abs.Body),
            App app when IsVal(app.Left) => new App(app.Left, Eval1(ctx, app.Right)),
            App app => new App(Eval1(ctx, app.Left), app.Right),
            _ => throw new NoRulesAppliesException()
        };
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
}