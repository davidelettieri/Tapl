using Common;
using System;
using static System.Console;
using static FullSimple.Core.Typing;
using FullSimple.Syntax.Terms;
using FullSimple.Syntax.Types;
using System.Linq;

namespace FullSimple.Syntax
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
                    var (c, x) = ctx.PickFreshName(abs.V);
                    Write("lambda {0} :", x);
                    PrintType(abs.Type);
                    Write(".");
                    _printTerm(c, abs.Body);
                    break;
                case App app:
                    Write("(");
                    _printTerm(ctx, app.Left);
                    Write(")");
                    Write("(");
                    _printTerm(ctx, app.Right);
                    Write(")");
                    break;
                case Ascribe ascribe:
                    Write("(");
                    _printTerm(ctx, ascribe.Term);
                    Write(" as ");
                    PrintType(ascribe.Type);
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
                case Var var:
                    Write(ctx.IndexToName(var.Index));
                    break;
                case StringTerm stringTerm:
                    Write("(\"");
                    Write(stringTerm.Value);
                    Write("\")");
                    break;
                case True _:
                    Write("true");
                    break;
                case False _:
                    Write("false");
                    break;
                case Unit _:
                    Write("unit");
                    break;
                case Float f:
                    Write(f.Value);
                    break;
                case Record rec:
                    Write("{");
                    for (int i = 0; i < rec.Fields.Count; i++)
                    {
                        var field = rec.Fields[i];
                        if (field.Item1 != i.ToString())
                            Write(field.Item1 + "=");
                        _printTerm(ctx, field.Item2);
                        if (i < rec.Fields.Count - 1)
                            Write(",");
                    }
                    Write("}");
                    break;
                case Succ succ:
                    Write("succ ");
                    _printTerm(ctx, succ.Of);
                    break;
                case Zero:
                    Write("0");
                    break;
                default:
                    throw new InvalidOperationException();
            }
        }

        private static void PrintType(IType type)
        {
            switch (type)
            {
                case TypeNat:
                    Write("Nat");
                    break;
                case TypeString:
                    Write("String");
                    break;
                case TypeBool:
                    Write("Bool");
                    break;
                case TypeArrow t:
                    PrintType(t.From);
                    Write(" -> ");
                    PrintType(t.To);
                    break;
                case TypeVariant tv:
                    Write("<");
                    var enumerator = tv.Variants.GetEnumerator();
                    if (enumerator.MoveNext())
                    {
                        var current = enumerator.Current;
                        Write($"{current.Item1}:");
                        PrintType(current.Item2);
                        while (enumerator.MoveNext())
                        {
                            current = enumerator.Current;
                            Write(",");
                            Write($"{current.Item1}:");
                            PrintType(current.Item2);
                        }
                    }
                    Write(">");
                    break;
                case TypeUnit _:
                    Write("Unit");
                    break;
                case TypeId ti:
                    Write(ti.Name);
                    break;
                case TypeRecord trec:
                    Write("{");
                    var source = trec.Variants.ToList();
                    for (int i = 0; i < source.Count; i++)
                    {
                        var si = source[i];
                        if (si.Item1 != i.ToString())
                            Write(si.Item1 + "=");
                        PrintType(si.Item2);
                        if (i < source.Count - 1)
                            Write(",");
                    }
                    Write("}");
                    break;
                default:
                    throw new InvalidOperationException();
            }
        }
    }
}
