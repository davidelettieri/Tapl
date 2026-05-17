using Common;
using FullUpdate.Syntax;
using FullUpdate.Syntax.Terms;
using static FullUpdate.Core.Shifting;

namespace FullUpdate.Core;

public static class Substitution
{
    // Term substitution
    public static ITerm TermSubst(int j, ITerm s, ITerm t)
        => TmMap(
            (j, v) => v.Index == j ? TermShift(j, s) : v,
            (_, ty) => ty,
            j, t);

    public static ITerm TermSubstTop(ITerm s, ITerm t)
        => TermShift(-1, TermSubst(0, TermShift(1, s), t));

    // Type substitution within types
    private static IType TypeSubst(IType tyS, int j, IType tyT)
        => TypeMap((j, tv) => tv.X == j ? TypeShift(j, tyS) : tv, j, tyT);

    public static IType TypeSubstTop(IType tyS, IType tyT)
        => TypeShift(-1, TypeSubst(TypeShift(1, tyS), 0, tyT));

    // Type substitution within terms
    private static ITerm TyTermSubst(IType tyS, int j, ITerm t)
        => TmMap(
            (_, v) => v,
            (j, ty) => TypeSubst(tyS, j, ty),
            j, t);

    public static ITerm TyTermSubstTop(IType tyS, ITerm t)
        => TermShift(-1, TyTermSubst(TypeShift(1, tyS), 0, t));
}
