using System;

namespace Common;

public static class DeBruijnTermOperations
{
    public static TTerm TermShiftAbove<TTerm, TVariable>(
        int d,
        int c,
        TTerm t,
        IDeBruijnTermAdapter<TTerm, TVariable> adapter)
    {
        TTerm OnVar(int cutoff, TVariable variable)
        {
            var index = adapter.GetIndex(variable);
            var shiftedIndex = index >= cutoff ? index + d : index;
            var shiftedContextLength = adapter.GetContextLength(variable) + d;

            return adapter.CreateShiftedVar(variable, shiftedIndex, shiftedContextLength);
        }

        return adapter.Map(OnVar, c, t);
    }

    public static TTerm TermShift<TTerm, TVariable>(
        int d,
        TTerm t,
        IDeBruijnTermAdapter<TTerm, TVariable> adapter)
        => TermShiftAbove(d, 0, t, adapter);

    public static TTerm TermSubst<TTerm, TVariable>(
        int j,
        TTerm s,
        TTerm t,
        IDeBruijnTermAdapter<TTerm, TVariable> adapter,
        Func<int, TTerm, TTerm> termShift)
    {
        TTerm OnVar(int depth, TVariable variable)
            => adapter.GetIndex(variable) == j + depth
                ? termShift(depth, s)
                : adapter.ToTerm(variable);

        return adapter.Map(OnVar, 0, t);
    }

    public static TTerm TermSubstTop<TTerm, TVariable>(
        TTerm s,
        TTerm t,
        IDeBruijnTermAdapter<TTerm, TVariable> adapter,
        Func<int, TTerm, TTerm> termShift)
        => termShift(-1, TermSubst(0, termShift(1, s), t, adapter, termShift));
}