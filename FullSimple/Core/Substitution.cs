using FullSimple.Syntax;
using Common;
using static FullSimple.Core.Shifting;
using FullSimple.Syntax.Terms;

namespace FullSimple.Core
{
    public static class Substitution
    {
        public static ITerm TermSubst(int j, ITerm s, ITerm t)
        {
            ITerm f(int j, Var v) => v.Index == j ? TermShift(j, s) : v;

            return TmMap(f, j, t);
        }

        public static ITerm TermSubsTop(ITerm s, ITerm t)
            => TermShift(-1, TermSubst(0, TermShift(1, s), t));
    }
}
