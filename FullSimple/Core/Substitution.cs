using Common;
using static FullSimple.Core.Shifting;
using FullSimple.Syntax.Terms;
using FullSimple.Syntax.Types;

namespace FullSimple.Core
{
    public static class Substitution
    {
        public static ITerm TermSubst(int j, ITerm s, ITerm t)
        {
            ITerm f(int j, Var v) => v.Index == j ? TermShift(j, s) : v;
            IType g(int _, IType t) => t;
            return TmMap(f, g, j, t);
        }

        public static ITerm TermSubsTop(ITerm s, ITerm t)
            => TermShift(-1, TermSubst(0, TermShift(1, s), t));

        public static IType TypeSubs(IType tyS, int j, IType tyT)
        {
            IType f(int j, TypeVar tv) => tv.X == j ? TypeShift(j, tyS) : tv;

            return TypeMap(f, j, tyT);
        }

        public static IType TypeSubsTop(IType s, IType t)
            => TypeShift(-1, TypeSubs(TypeShift(1, s), 0, t));

        public static ITerm TyTermSubst(IType tyS, int j, ITerm t)
        {
            ITerm f(int _, Var v) => v;
            IType g(int j, IType t) => TypeSubs(tyS, j, t);

            return TmMap(f, g, j, t);
        }

        public static ITerm TypeTermSubsTop(IType tyS, ITerm t)
            => TermShift(-1, TyTermSubst(TypeShift(1, tyS), 0, t));
    }
}
