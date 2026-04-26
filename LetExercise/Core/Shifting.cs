using System;
using Common;
using LetExercise.Syntax;

namespace LetExercise.Core;

public static class Shifting
{
    internal static IDeBruijnTermAdapter<ITerm, Var> Adapter { get; } = new LetExerciseDeBruijnTermAdapter();

    public static ITerm TmMap(Func<int, Var, ITerm> onVar, int c, ITerm t)
    {
        ITerm Walk(int c, ITerm t)
        {
            return t switch
            {
                Let let => new Let(let.Info, let.Variable, Walk(c, let.LetTerm), Walk(c + 1, let.InTerm)),
                Var var => onVar(c, var),
                Abs abs => new Abs(abs.Info, Walk(c + 1, abs.Body), abs.BoundedVariable, abs.Type),
                App app => new App(app.Info, Walk(c, app.Left), Walk(c, app.Right)),
                True e => e,
                False f => f,
                If ift => new If(ift.Info, Walk(c, ift.Condition), Walk(c, ift.Then), Walk(c, ift.Else)),
                _ => throw new InvalidOperationException()
            };
        }

        return Walk(c, t);
    }

    public static ITerm TermShift(int d, ITerm t) => DeBruijnTermOperations.TermShift(d, t, Adapter);

    private sealed class LetExerciseDeBruijnTermAdapter : IDeBruijnTermAdapter<ITerm, Var>
    {
        public ITerm Map(Func<int, Var, ITerm> onVar, int c, ITerm term) => TmMap(onVar, c, term);

        public int GetIndex(Var variable) => variable.Index;

        public int GetContextLength(Var variable) => variable.ContextLength;

        public ITerm ToTerm(Var variable) => variable;

        public ITerm CreateShiftedVar(Var variable, int shiftedIndex, int shiftedContextLength)
            => new Var(variable.Info, shiftedIndex, shiftedContextLength);
    }
}