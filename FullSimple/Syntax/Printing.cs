using Common;
using System;
using static System.Console;
using static FullSimple.Core.Typing;
using FullSimple.Syntax.Terms;
using System.Linq;
using FullSimple.Syntax.Bindings;
using System.Globalization;

namespace FullSimple.Syntax;

public static class Printing
{
    public static void PrintBindingType(Context ctx, IBinding b, string name)
    {
            Write(name);
            switch (b)
            {
                case VarBind vb:
                    PrintType(ctx, vb.Type);
                    break;
                case TermAbbBind { Type: not null } tab:
                    PrintType(ctx, tab.Type);
                    break;
                case TermAbbBind tab:
                    PrintType(ctx, TypeOf(ctx, tab.Term));
                    break;
                case TypeAbbBind:
                    Write(":: *");
                    break;
            }
            WriteLine();
        }

    public static void PrintTerm(Context ctx, ITerm t)
    {
            _printTerm(ctx, t);
        }

    private static void _printTerm(Context ctx, ITerm t)
    {
            switch (t)
            {
                case Abs abs:
                    var (c, x) = ctx.PickFreshName(abs.V);
                    Write("lambda {0} :", x);
                    PrintType(ctx, abs.Type);
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
                    PrintType(ctx, ascribe.Type);
                    Write(")");
                    break;
                case Case _case:
                    Write("case ");
                    _printTerm(ctx, _case.Term);
                    Write(" of ");
                    var source = _case.Cases.ToList();
                    for (int i = 0; i < source.Count; i++)
                    {
                        var (label, variable, term) = source[i];
                        pc(label, variable, term);
                        if (i < source.Count - 1)
                            Write("|");
                    }
                    void pc(string li, string xi, ITerm ti)
                    {
                        var (ctx1, _) = ctx.PickFreshName(xi);
                        Write($"<{li}={xi}>==>");
                        _printTerm(ctx1, ti);
                    }
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
                    Write(stringTerm.Value);
                    break;
                case True:
                    Write("true");
                    break;
                case False:
                    Write("false");
                    break;
                case Unit:
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
                    PrintSucc(succ, 1);

                    void PrintSucc(Succ c, int i)
                    {
                        switch (c.Of)
                        {
                            case Zero:
                                Write(i.ToString(CultureInfo.InvariantCulture));
                                break;
                            case Succ s:
                                PrintSucc(s, i + 1);
                                break;
                            default:
                                Write("succ ");
                                _printTerm(ctx, c.Of);
                                break;
                        }
                    }
                    break;
                case Pred pred:
                    Write("pred (");
                    _printTerm(ctx, pred.Of);
                    Write(")");
                    break;
                case Zero:
                    Write("0");
                    break;
                case Proj proj:
                    _printTerm(ctx, proj.Term);
                    Write(".");
                    Write(proj.Label);
                    break;
                default:
                    throw new InvalidOperationException();
            }
        }

    public static void PrintType(Context ctx, IType type, bool startWithColon = true, bool addNewline = false)
    {
            if (startWithColon)
                Write(" : ");

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
                    PrintType(ctx, t.From, false);
                    Write(" -> ");
                    PrintType(ctx, t.To, false);
                    break;
                case TypeVariant tv:
                    Write("<");
                    var enumerator = tv.Variants.GetEnumerator();
                    if (enumerator.MoveNext())
                    {
                        var current = enumerator.Current;
                        Write($"{current.Item1}:");
                        PrintType(ctx, current.Item2);
                        while (enumerator.MoveNext())
                        {
                            current = enumerator.Current;
                            Write(",");
                            Write($"{current.Item1}:");
                            PrintType(ctx, current.Item2);
                        }
                    }
                    Write(">");
                    break;
                case TypeUnit:
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
                            Write(si.Item1);
                        PrintType(ctx, si.Item2);
                        if (i < source.Count - 1)
                            Write(",");
                    }
                    Write("}");
                    break;
                case TypeVar tv:
                    if (ctx.Length == tv.N)
                        Write(ctx.IndexToName(tv.X));
                    else
                        Write("[bad index]");
                    break;
                default:
                    throw new InvalidOperationException();
            }

            if (addNewline)
                WriteLine();
        }
}