using Common;
using FullRecon.Syntax.Terms;
using static FullRecon.Core.Shifting;

namespace FullRecon.Core;

public static class Substitution
{
    private static ITerm TermSubst(int j, ITerm s, ITerm t)
    {
        ITerm OnVar(int j, Var v) =>
            v.Index == j ? TermShift(j, s) : new Var(v.Info, v.Index, v.ContextLength);

        return TmMap(OnVar, j, t);
    }

    public static ITerm TermSubstTop(ITerm s, ITerm t)
        => TermShift(-1, TermSubst(0, TermShift(1, s), t));
}
