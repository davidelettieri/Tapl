using Common;
using System;
using static System.Console;
using static LetExercise.Core.Typing;

namespace LetExercise.Syntax
{
    public static class Printing
    {
        public static void PrintTerm(Context ctx, ITerm t)
        {
            PrintTerm1(ctx, t);
            Write(" : ");
            PrintType(TypeOf(ctx, t));
            WriteLine();
        }

        private static void PrintTerm1(Context ctx, ITerm t)
        {
            switch (t)
            {
                case Let let:
                    Write($"( let {let.Variable} = ");
                    PrintTerm1(ctx, let.LetTerm);
                    Write(" in ");
                    PrintTerm1(ctx, let.InTerm);
                    Write(")");
                    break;
                case Abs abs:
                    var (c, x) = ctx.PickFreshName(abs.BoundedVariable);
                    Write("lambda {0} :", x);
                    PrintType(abs.Type);
                    Write(".(");
                    PrintTerm1(c, abs.Body);
                    Write(")");
                    break;
                case If ift:
                    Write("( if ");
                    PrintTerm1(ctx, ift.Condition);
                    Write(" then ");
                    PrintTerm1(ctx, ift.Then);
                    Write(" else ");
                    PrintTerm1(ctx, ift.Else);
                    Write(")");
                    break;
                case App app:
                    Write("(");
                    PrintTerm1(ctx, app.Left);
                    Write(")");
                    Write("(");
                    PrintTerm1(ctx, app.Right);
                    Write(")");
                    break;
                case Var var:
                    Write(ctx.IndexToName(var.Index));
                    break;
                case True _:
                    Write("true");
                    break;
                case False _:
                    Write("false");
                    break;
                default:
                    throw new InvalidOperationException();
            }
        }

        private static void PrintType(IType type)
        {
            switch (type)
            {
                case TypeBool _:
                    Write("Bool");
                    break;
                case TypeArrow t:
                    Write("(");
                    PrintType(t.From);
                    Write(" -> ");
                    PrintType(t.To);
                    Write(")");
                    break;
                default:
                    throw new InvalidOperationException();
            }
        }
    }
}
