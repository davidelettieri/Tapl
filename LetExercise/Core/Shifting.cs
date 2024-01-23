using LetExercise.Syntax;
using Common;
using System;

namespace LetExercise.Core;

public static class Shifting
{
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

    public static ITerm TermShiftAbove(int d, int c, ITerm t)
    {
            ITerm f(int c, Var v) =>
                v.Index >= c ? new Var(v.Info, v.Index + d, v.ContextLength + d) : new Var(v.Info, v.Index, v.ContextLength + d);

            return TmMap(f, c, t);
        }

    public static ITerm TermShift(int d, ITerm t) => TermShiftAbove(d, 0, t);
}