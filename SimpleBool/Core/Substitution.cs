using Common;
using SimpleBool.Syntax;
using static SimpleBool.Core.Shifting;

namespace SimpleBool.Core;

public static class Substitution
{
    public static ITerm TermSubst(int j, ITerm s, ITerm t)
        => DeBruijnTermOperations.TermSubst(j, s, t, Shifting.Adapter, TermShift);

    public static ITerm TermSubsTop(ITerm s, ITerm t)
        => DeBruijnTermOperations.TermSubstTop(s, t, Shifting.Adapter, TermShift);
}