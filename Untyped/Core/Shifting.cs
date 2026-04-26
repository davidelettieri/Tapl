using System;
using Common;
using Untyped.Terms;

namespace Untyped.Core;

public static class Shifting
{
    internal static IDeBruijnTermAdapter<ITerm, Var> Adapter { get; } = new UntypedDeBruijnTermAdapter();

    public static ITerm TmMap(Func<int, Var, ITerm> onVar, int c, ITerm t)
    {
        ITerm Walk(int depth, ITerm term)
        {
            return term switch
            {
                Var var => onVar(depth, var),
                Abs abs => new Abs(Walk(depth + 1, abs.Body), abs.BoundedVariable),
                App app => new App(Walk(depth, app.Left), Walk(depth, app.Right)),
                _ => throw new InvalidOperationException()
            };
        }

        return Walk(c, t);
    }

    public static ITerm TermShift(int d, ITerm t) => DeBruijnTermOperations.TermShift(d, t, Adapter);

    private sealed class UntypedDeBruijnTermAdapter : IDeBruijnTermAdapter<ITerm, Var>
    {
        public ITerm Map(Func<int, Var, ITerm> onVar, int c, ITerm term) => TmMap(onVar, c, term);

        public int GetIndex(Var variable) => variable.Index;

        public int GetContextLength(Var variable) => variable.ContextLength;

        public ITerm ToTerm(Var variable) => variable;

        public ITerm CreateShiftedVar(Var variable, int shiftedIndex, int shiftedContextLength)
            => new Var(shiftedIndex, shiftedContextLength);
    }
}