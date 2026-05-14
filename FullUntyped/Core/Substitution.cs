using Common;
using System;
using FullUntyped.Syntax;
using FullUntyped.Syntax.Terms;
using static FullUntyped.Core.Shifting;

namespace FullUntyped.Core;

public static class Substitution
{
    private static readonly IDeBruijnTermAdapter<ITerm, Var> Adapter = new FullUntypedSubstitutionAdapter();

    public static ITerm TermSubst(int j, ITerm s, ITerm t)
        => DeBruijnTermOperations.TermSubst(j, s, t, Adapter, TermShift);

    public static ITerm TermSubsTop(ITerm s, ITerm t)
        => DeBruijnTermOperations.TermSubstTop(s, t, Adapter, TermShift);

    private static IType TypeSubs(IType tyS, int j, IType tyT)
    {
        IType f(int j, TypeVar tv) => tv.X == j ? TypeShift(j, tyS) : tv;

        return TypeMap(f, j, tyT);
    }

    public static IType TypeSubsTop(IType s, IType t)
        => TypeShift(-1, TypeSubs(TypeShift(1, s), 0, t));

    private static ITerm TyTermSubst(IType tyS, int j, ITerm t)
    {
        ITerm f(int _, Var v) => v;
        IType g(int j, IType t) => TypeSubs(tyS, j, t);

        return TmMap(f, g, j, t);
    }

    public static ITerm TypeTermSubsTop(IType tyS, ITerm t)
        => TermShift(-1, TyTermSubst(TypeShift(1, tyS), 0, t));

    private sealed class FullUntypedSubstitutionAdapter : IDeBruijnTermAdapter<ITerm, Var>
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
}