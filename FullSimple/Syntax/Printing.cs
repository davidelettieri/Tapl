using System;
using System.Globalization;
using System.Linq;
using Common;
using FullSimple.Syntax.Bindings;
using FullSimple.Syntax.Terms;
using static FullSimple.Core.Typing;

namespace FullSimple.Syntax;

public static class Printing
{
    private static bool Small(ITerm t) => t is Var;

    public static void PrintBindingType(Context ctx, IBinding b, string name)
    {
        var pp = new PrettyPrinter();
        PrintBindingType(pp, ctx, b, name);
        Console.Write(pp.ToString());
    }

    private static void PrintBindingType(PrettyPrinter pp, Context ctx, IBinding b, string name)
    {
        pp.Write(name);
        switch (b)
        {
            case VarBind vb:
                PrintType(pp, ctx, vb.Type);
                break;
            case TermAbbBind { Type: not null } tab:
                PrintType(pp, ctx, tab.Type);
                break;
            case TermAbbBind tab:
                PrintType(pp, ctx, TypeOf(ctx, tab.Term));
                break;
            case TypeAbbBind:
                pp.Write(":: *");
                break;
        }

        pp.NewLine();
    }

    public static void PrintTerm(Context ctx, ITerm t)
    {
        var pp = new PrettyPrinter();
        PrintTerm(pp, ctx, t);
        Console.Write(pp.ToString());
    }

    public static void PrintATerm(Context ctx, ITerm t)
    {
        var pp = new PrettyPrinter();
        PrintTmATerm(pp, true, ctx, t);
        Console.Write(pp.ToString());
    }

    private static void PrintTerm(PrettyPrinter pp, Context ctx, ITerm t)
    {
        PrintTmTerm(pp, true, ctx, t);
    }

    private static void PrintTmATerm(PrettyPrinter pp, bool outer, Context ctx, ITerm t)
    {
        switch (t)
        {
            case StringTerm stringTerm:
                pp.Write("\"");
                pp.Write(stringTerm.Value);
                pp.Write("\"");
                break;
            case True:
                pp.Write("true");
                break;
            case False:
                pp.Write("false");
                break;
            case Tag tag:
                pp.Obox();
                pp.Write("<");
                pp.Write(tag.Label);
                pp.Write("=");
                PrintTmTerm(pp, false, ctx, tag.Term);
                pp.Write(">");
                pp.PrintSpace();
                pp.Write("as ");
                PrintType(pp, ctx, tag.Type, false);
                pp.Cbox();
                break;
            case Unit:
                pp.Write("unit");
                break;
            case Var var:
                pp.Write(ctx.IndexToName(var.Index));
                break;
            case Float f:
                pp.Write(f.Value.ToString(CultureInfo.InvariantCulture));
                break;
            case Record:
            case Inert:
            case Zero:
            case Succ:
                PrintTmTerm(pp, outer, ctx, t);
                break;
            default:
                pp.Write("(");
                PrintTmTerm(pp, outer, ctx, t);
                pp.Write(")");
                break;
        }
    }

    private static void PrintTmTerm(PrettyPrinter pp, bool outer, Context ctx, ITerm t)
    {
        switch (t)
        {
            case If ift:
                pp.Obox();
                pp.Write("if ");
                PrintTmTerm(pp, false, ctx, ift.Condition);
                pp.PrintSpace();
                pp.Write("then ");
                PrintTmTerm(pp, false, ctx, ift.Then);
                pp.PrintSpace();
                pp.Write("else ");
                PrintTmTerm(pp, false, ctx, ift.Else);
                pp.Cbox();
                break;

            case Case @case:
                pp.Obox();
                pp.Write("case ");
                PrintTmTerm(pp, false, ctx, @case.Term);
                pp.Write(" of");
                pp.PrintSpace();
                var source = @case.Cases.ToList();
                for (int i = 0; i < source.Count; i++)
                {
                    var (label, variable, term) = source[i];
                    pc(label, variable, term);
                    if (i < source.Count - 1)
                        pp.Write("|");
                }
                pp.CloseBox();
                void pc(string li, string xi, ITerm ti)
                {
                    var (ctx1, xi1) = ctx.PickFreshName(xi);
                    pp.Write("<");
                    pp.Write(li);
                    pp.Write("=");
                    pp.Write(xi1);
                    pp.Write(">==>");
                    PrintTmTerm(pp, false, ctx1, ti);

                }

                break;
            case Let let:
                pp.Obox();
                pp.Write("let ");
                pp.Cbox();
                PrintTmTerm(pp, false, ctx, let.LetTerm);
                pp.PrintSpace();
                pp.Write("in");
                pp.PrintSpace();
                PrintTmTerm(pp, false, ctx.AddName(let.Variable), let.InTerm);
                pp.Cbox();
                break;
            case Abs abs:
                var (ctxp, xp) = ctx.PickFreshName(abs.V);
                pp.Obox();
                pp.Write("lambda ");
                pp.Write(xp);
                pp.Write(":");
                PrintType(pp, ctx, abs.Type, false);
                pp.Write(".");
                if (Small(abs.Body) && !outer)
                {
                    pp.PrintBreak(0,0);
                }
                else
                {
                    pp.PrintSpace();
                }
                PrintTmTerm(pp, outer, ctxp, abs.Body);       
                pp.Cbox();         
                break;
            case Fix fix:
                pp.Obox();
                pp.Write("fix ");
                PrintTmTerm(pp, false, ctx, fix.Term);
                pp.Cbox();
                break;
            case TimesFloat timesFloat:
                pp.Write("timesfloat ");
                PrintTmTerm(pp, false, ctx, timesFloat.Left);
                pp.PrintSpace();
                PrintTmTerm(pp, false, ctx, timesFloat.Right);
                break;
            case App app:
                pp.Write("(");
                PrintTmTerm(pp, false, ctx, app.Left);
                pp.Write(")");
                pp.Write("(");
                PrintTmTerm(pp, false, ctx, app.Right);
                pp.Write(")");
                break;
            case Ascribe ascribe:
                pp.Write("(");
                PrintTmTerm(pp, false, ctx, ascribe.Term);
                pp.Write(" as ");
                PrintType(pp, ctx, ascribe.Type);
                pp.Write(")");
                break;
            case Var var:
                pp.Write(ctx.IndexToName(var.Index));
                break;
            case StringTerm stringTerm:
                pp.Write(stringTerm.Value);
                break;
            case True:
                pp.Write("true");
                break;
            case False:
                pp.Write("false");
                break;
            case Unit:
                pp.Write("unit");
                break;
            case Float f:
                pp.Write(f.Value.ToString(CultureInfo.InvariantCulture));
                break;
            case Record rec:
                pp.Write("{");
                for (int i = 0; i < rec.Fields.Count; i++)
                {
                    var field = rec.Fields[i];
                    if (field.Item1 != i.ToString())
                        pp.Write(field.Item1 + "=");
                    PrintTmTerm(pp, false, ctx, field.Item2);
                    if (i < rec.Fields.Count - 1)
                        pp.Write(",");
                }

                pp.Write("}");
                break;
            case Succ succ:
                PrintSucc(succ, 1);

                void PrintSucc(Succ sc, int i)
                {
                    switch (sc.Of)
                    {
                        case Zero:
                            pp.Write(i.ToString(CultureInfo.InvariantCulture));
                            break;
                        case Succ s:
                            PrintSucc(s, i + 1);
                            break;
                        default:
                            pp.Write("succ ");
                            PrintTmTerm(pp, false, ctx, sc.Of);
                            break;
                    }
                }

                break;
            case Pred pred:
                pp.Write("pred (");
                PrintTmTerm(pp, false, ctx, pred.Of);
                pp.Write(")");
                break;
            case IsZero isZero:
                pp.Write("iszero ");
                PrintTmTerm(pp, false, ctx, isZero.Term);
                break;
            case Zero:
                pp.Write("0");
                break;
            case Proj proj:
                PrintTmTerm(pp, false, ctx, proj.Term);
                pp.Write(".");
                pp.Write(proj.Label);
                break;
            default:
                throw new InvalidOperationException();
        }
    }

    public static void PrintType(Context ctx, IType type, bool startWithColon = true, bool addNewline = false)
    {
        var pp = new PrettyPrinter();
        PrintType(pp, ctx, type, startWithColon, addNewline);
        Console.Write(pp.ToString());
    }

    private static void PrintType(PrettyPrinter pp, Context ctx, IType type, bool startWithColon = true, bool addNewline = false)
    {
        if (startWithColon)
            pp.Write(" : ");

        switch (type)
        {
            case TypeNat:
                pp.Write("Nat");
                break;
            case TypeString:
                pp.Write("String");
                break;
            case TypeFloat:
                pp.Write("Float");
                break;
            case TypeBool:
                pp.Write("Bool");
                break;
            case TypeArrow t:
                PrintType(pp, ctx, t.From, false);
                pp.Write(" -> ");
                PrintType(pp, ctx, t.To, false);
                break;
            case TypeVariant tv:
                pp.Write("<");
                using (var enumerator = tv.Variants.GetEnumerator())
                {
                    if (enumerator.MoveNext())
                    {
                        var current = enumerator.Current;
                        pp.Write($"{current.Item1}:");
                        PrintType(pp, ctx, current.Item2);
                        while (enumerator.MoveNext())
                        {
                            current = enumerator.Current;
                            pp.Write(",");
                            pp.Write($"{current.Item1}:");
                            PrintType(pp, ctx, current.Item2);
                        }
                    }
                }

                pp.Write(">");
                break;
            case TypeUnit:
                pp.Write("Unit");
                break;
            case TypeId ti:
                pp.Write(ti.Name);
                break;
            case TypeRecord trec:
                pp.Write("{");
                var source = trec.Variants.ToList();
                for (int i = 0; i < source.Count; i++)
                {
                    var si = source[i];
                    if (si.Item1 != i.ToString())
                        pp.Write(si.Item1);
                    PrintType(pp, ctx, si.Item2);
                    if (i < source.Count - 1)
                        pp.Write(",");
                }

                pp.Write("}");
                break;
            case TypeVar tv:
                pp.Write(ctx.Length == tv.N ? ctx.IndexToName(tv.X) : "[bad index]");
                break;
            default:
                throw new InvalidOperationException();
        }

        if (addNewline)
            pp.NewLine();
    }

    public static void PrintBinding(PrettyPrinter pp, Context ctx, IBinding bind)
    {
        switch (bind)
        {
            case NameBinding:
                break;
            case TermAbbBind tab:
                pp.Write("= ");
                PrintTerm(pp, ctx, tab.Term);
                break;
            case VarBind vb:
                pp.Write(": ");
                PrintType(pp, ctx, vb.Type);
                break;
            case TypeVarBind:
                break;
            case TypeAbbBind tab:
                pp.Write("= ");
                PrintType(pp, ctx, tab.Type);
                break;
        }
    }
}
