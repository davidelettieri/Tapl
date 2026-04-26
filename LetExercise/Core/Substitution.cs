using Common;
using LetExercise.Syntax;
using static LetExercise.Core.Shifting;

namespace LetExercise.Core;

public static class Substitution
{
    public static ITerm TermSubst(int j, ITerm s, ITerm t)
        => DeBruijnTermOperations.TermSubst(j, s, t, Shifting.Adapter, TermShift);

    public static ITerm TermSubsTop(ITerm s, ITerm t)
        => DeBruijnTermOperations.TermSubstTop(s, t, Shifting.Adapter, TermShift);
}