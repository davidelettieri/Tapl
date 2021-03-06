﻿using LetExercise.Syntax;
using Common;
using static LetExercise.Core.Shifting;
using static LetExercise.Core.Substitution;

namespace LetExercise.Core
{
    public static class Evaluation
    {
        public static bool IsVal(Context _, ITerm t)
        {
            return t switch
            {
                True _ => true,
                False _ => true,
                Abs _ => true,
                _ => false
            };
        }

        private static ITerm Eval1(Context ctx, ITerm t)
        {
            return t switch
            {
                Let let when IsVal(ctx, let.LetTerm) => TermSubsTop(let.LetTerm, let.InTerm),
                Let let => new Let(let.Info, let.Variable, Eval1(ctx, let.LetTerm), let.InTerm),
                App app when app.Left is Abs abs && IsVal(ctx, app.Right)
                    => TermSubsTop(app.Right, abs.Body),
                App app when IsVal(ctx, app.Left) => new App(app.Info, app.Left, Eval1(ctx, app.Right)),
                App app => new App(app.Info, Eval1(ctx, app.Left), app.Right),
                If ift when ift.Condition is True => ift.Then,
                If ift when ift.Condition is False => ift.Else,
                If ift => new If(ift.Info, Eval1(ctx, ift.Condition), ift.Then, ift.Else),
                _ => throw new NoRulesAppliesException()
            };
        }

        public static ITerm Eval(Context ctx, ITerm t)
        {
            try
            {
                var t1 = Eval1(ctx, t);

                return Eval(ctx, t1);
            }
            catch (NoRulesAppliesException)
            {
                return t;
            }
        }

        public static ITerm TermSubsTop(ITerm s, ITerm t)
            => TermShift(-1, TermSubst(0, TermShift(1, s), t));
    }
}
