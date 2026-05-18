using Common;
using FullRecon.Syntax.Terms;
using static FullRecon.Core.Substitution;

namespace FullRecon.Core;

public static class Evaluation
{
    private static bool IsNumericVal(Context ctx, ITerm t) => t switch
    {
        Zero => true,
        Succ s => IsNumericVal(ctx, s.Of),
        _ => false
    };

    public static bool IsVal(Context ctx, ITerm t) => t switch
    {
        True => true,
        False => true,
        Abs => true,
        _ when IsNumericVal(ctx, t) => true,
        _ => false
    };

    private static ITerm Eval1(Context ctx, ITerm t) => t switch
    {
        App { Left: Abs abs } app when IsVal(ctx, app.Right) =>
            TermSubstTop(app.Right, abs.Body),
        App app when IsVal(ctx, app.Left) =>
            new App(app.Info, app.Left, Eval1(ctx, app.Right)),
        App app =>
            new App(app.Info, Eval1(ctx, app.Left), app.Right),
        If { Condition: True } ift => ift.Then,
        If { Condition: False } ift => ift.Else,
        If ift => new If(ift.Info, Eval1(ctx, ift.Condition), ift.Then, ift.Else),
        Succ s => new Succ(s.Info, Eval1(ctx, s.Of)),
        Pred { Of: Zero } => new Zero(new UnknownInfo()),
        Pred { Of: Succ s } when IsNumericVal(ctx, s.Of) => s.Of,
        Pred p => new Pred(p.Info, Eval1(ctx, p.Of)),
        IsZero { Term: Zero } => new True(new UnknownInfo()),
        IsZero { Term: Succ s } when IsNumericVal(ctx, s.Of) => new False(new UnknownInfo()),
        IsZero iz => new IsZero(iz.Info, Eval1(ctx, iz.Term)),
        Let let when IsVal(ctx, let.LetTerm) => TermSubstTop(let.LetTerm, let.InTerm),
        Let let => new Let(let.Info, let.Variable, Eval1(ctx, let.LetTerm), let.InTerm),
        _ => throw new NoRulesAppliesException()
    };

    public static ITerm Eval(Context ctx, ITerm t)
    {
        try
        {
            return Eval(ctx, Eval1(ctx, t));
        }
        catch (NoRulesAppliesException)
        {
            return t;
        }
    }
}
