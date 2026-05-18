using System;
using Common;
using FullRecon.Syntax.Terms;

namespace FullRecon.Core;

public static class Shifting
{
    public static ITerm TmMap(Func<int, Var, ITerm> onVar, int c, ITerm t)
    {
        ITerm Walk(int c, ITerm t) => t switch
        {
            Var var => onVar(c, var),
            Abs abs => new Abs(abs.Info, abs.V, abs.Type, Walk(c + 1, abs.Body)),
            App app => new App(app.Info, Walk(c, app.Left), Walk(c, app.Right)),
            True tr => tr,
            False f => f,
            If ift => new If(ift.Info, Walk(c, ift.Condition), Walk(c, ift.Then), Walk(c, ift.Else)),
            Zero z => z,
            Succ s => new Succ(s.Info, Walk(c, s.Of)),
            Pred p => new Pred(p.Info, Walk(c, p.Of)),
            IsZero iz => new IsZero(iz.Info, Walk(c, iz.Term)),
            Let let => new Let(let.Info, let.Variable, Walk(c, let.LetTerm), Walk(c + 1, let.InTerm)),
            _ => throw new InvalidOperationException($"Unexpected term: {t}")
        };

        return Walk(c, t);
    }

    private static ITerm TermShiftAbove(int d, int c, ITerm t)
    {
        ITerm OnVar(int c, Var v) =>
            v.Index >= c
                ? new Var(v.Info, v.Index + d, v.ContextLength + d)
                : new Var(v.Info, v.Index, v.ContextLength + d);

        return TmMap(OnVar, c, t);
    }

    public static ITerm TermShift(int d, ITerm t) => TermShiftAbove(d, 0, t);
}
