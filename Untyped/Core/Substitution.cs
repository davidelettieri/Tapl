using Common;
using Untyped.Terms;
using static Untyped.Core.Shifting;

namespace Untyped.Core;

public static class Substitution
{
    public static ITerm TermSubst(int j, ITerm s, ITerm t)
        => DeBruijnTermOperations.TermSubst(j, s, t, Adapter, TermShift);

    public static ITerm TermSubsTop(ITerm s, ITerm t)
        => DeBruijnTermOperations.TermSubstTop(s, t, Adapter, TermShift);
}