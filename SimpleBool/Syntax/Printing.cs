using Common;
using System;
using System.Collections.Generic;
using System.Security;
using System.Text;
using static System.Console;
using static SimpleBool.Core.Typing;

namespace SimpleBool.Syntax
{
    public static class Printing
    {
        public static void PrintTerm(Context ctx, ITerm t)
        {
            _printTerm(ctx, t);
            Write(" : ");
            PrintType(TypeOf(ctx, t));
            WriteLine();
        }

        private static void _printTerm(Context ctx, ITerm t)
        {
            switch (t)
            {
                case Abs abs:
                    var (c, x) = ctx.PickFreshName(abs.BoundedVariable);
                    Write("lambda {0} :", x);
                    PrintType(abs.Type);
                    Write(".(");
                    _printTerm(c, abs.Body);
                    Write(")");
                    break;
                case If ift:
                    Write("( if ");
                    _printTerm(ctx, ift.Condition);
                    Write(" then ");
                    _printTerm(ctx, ift.Then);
                    Write(" else ");
                    _printTerm(ctx, ift.Else);
                    Write(")");
                    break;
                case App app:
                    Write("(");
                    _printTerm(ctx, app.Left);
                    Write(")");
                    Write("(");
                    _printTerm(ctx, app.Right);
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
