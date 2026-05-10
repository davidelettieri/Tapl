using System;
using Common;
using FullError.Syntax.Terms;
using static FullError.Core.Shifting;

namespace FullError.Core;

public static class Substitution
{
    private sealed class FullErrorSubstitutionAdapter : IDeBruijnTermAdapter<ITerm, Var>
    {
        public ITerm Map(Func<int, Var, ITerm> onVar, int c, ITerm term)
        {
            IType OnType(int _, IType type) => type;
            return TmMap(onVar, OnType, c, term);
        }

        public int GetIndex(Var variable) => variable.Index;

        public int GetContextLength(Var variable) => variable.ContextLength;

        public ITerm ToTerm(Var variable) => variable;

        public ITerm CreateShiftedVar(Var variable, int shiftedIndex, int shiftedContextLength)
            => new Var(variable.Info, shiftedIndex, shiftedContextLength);
    }

    private static readonly IDeBruijnTermAdapter<ITerm, Var> Adapter = new FullErrorSubstitutionAdapter();

    public static ITerm TermSubst(int j, ITerm s, ITerm t)
        => DeBruijnTermOperations.TermSubst(j, s, t, Adapter, TermShift);

    public static ITerm TermSubstTop(ITerm s, ITerm t)
        => DeBruijnTermOperations.TermSubstTop(s, t, Adapter, TermShift);
}
