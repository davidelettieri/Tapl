using System;
using System.Globalization;
using System.Linq;
using Common;
using FullPoly.Syntax.Bindings;
using FullPoly.Syntax.Terms;
using static FullPoly.Core.Typing;

namespace FullPoly.Syntax;

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
                pp.Write(" : ");
                PrintType(pp, ctx, vb.Type);
                break;
            case TermAbbBind { Type: not null } tab:
                pp.Write(" : ");
                PrintType(pp, ctx, tab.Type);
                break;
            case TermAbbBind tab:
                pp.Write(" : ");
                PrintType(pp, ctx, TypeOf(ctx, tab.Term));
                break;
            case TypeAbbBind:
                pp.Write(" :: *");
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

    public static void PrintTmATerm(Context ctx, ITerm t)
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
                PrintType(pp, ctx, tag.Type);
                pp.Cbox();
                break;
            case Pack pack:
                pp.Obox();
                pp.Write("{*");
                PrintType(pp, ctx, pack.WitnessType);
                pp.Write(",");
                if (outer) pp.PrintSpace(); else pp.PrintBreak(0, 0);
                PrintTmTerm(pp, false, ctx, pack.Term);
                pp.Write("}");
                pp.PrintSpace();
                pp.Write("as ");
                PrintType(pp, ctx, pack.ExistType);
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
            case Record record:
                pp.Obox0();
                pp.Write("{");
                var fields = record.Fields.ToList();
                for (int i = 0; i < fields.Count; i++)
                {
                    var (label, term) = fields[i];
                    if (label != i.ToString())
                    {
                        pp.Write(label);
                        pp.Write("=");
                    }

                    PrintTmTerm(pp, false, ctx, term);
                    if (i < fields.Count - 1)
                    {
                        pp.Write(",");
                        if (outer)
                        {
                            pp.PrintSpace();
                        }
                        else
                        {
                            pp.PrintBreak(0, 0);
                        }
                    }
                }

                pp.Write("}");
                pp.Cbox();
                break;
            case Inert inert:
                pp.Write("inert[");
                PrintType(pp, ctx, inert.Type);
                pp.Write("]");
                break;
            case Zero:
                pp.Write("0");
                break;
            case Succ succ:
                PrintNat(pp, ctx, succ, 0);
                break;
            default:
                pp.Write("(");
                PrintTmTerm(pp, outer, ctx, t);
                pp.Write(")");
                break;
        }
    }

    private static void PrintNat(PrettyPrinter pp, Context ctx, ITerm t, int value = 1)
    {
        switch (t)
        {
            case Zero:
                pp.Write(value.ToString(CultureInfo.InvariantCulture));
                break;
            case Succ succ:
                PrintNat(pp, ctx, succ.Of, value + 1);
                break;
            default:
                pp.Write("(succ ");
                PrintTmATerm(pp, false, ctx, t);
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
                pp.Write(let.Variable);
                pp.Write(" = ");
                PrintTmTerm(pp, false, ctx, let.LetTerm);
                pp.PrintSpace();
                pp.Write("in");
                pp.PrintSpace();
                PrintTmTerm(pp, false, ctx.AddName(let.Variable), let.InTerm);
                pp.Cbox();
                break;
            case Unpack unpack:
                var (ctxu1, tyy) = ctx.PickFreshName(unpack.TypeVar);
                var (ctxu2, xx) = ctxu1.PickFreshName(unpack.V);
                pp.Obox();
                pp.Write("let {");
                pp.Write(tyy);
                pp.Write(",");
                pp.Write(xx);
                pp.Write("} =");
                pp.PrintSpace();
                PrintTmTerm(pp, false, ctx, unpack.Package);
                pp.Write(" in ");
                PrintTmTerm(pp, outer, ctxu2, unpack.Body);
                pp.Cbox();
                break;
            case Abs abs:
                var (ctxp, xp) = ctx.PickFreshName(abs.V);
                pp.Obox();
                pp.Write("lambda ");
                pp.Write(xp);
                pp.Write(":");
                PrintType(pp, ctx, abs.Type);
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
            case TAbs tabs:
                var (ctxt1, tyx) = ctx.PickFreshName(tabs.TypeVar);
                pp.Obox();
                pp.Write("lambda ");
                pp.Write(tyx);
                pp.Write(".");
                if (Small(tabs.Body) && !outer) pp.PrintBreak(0, 0); else pp.PrintSpace();
                PrintTmTerm(pp, outer, ctxt1, tabs.Body);
                pp.Cbox();
                break;
            case Fix fix:
                pp.Obox();
                pp.Write("fix ");
                PrintTmTerm(pp, false, ctx, fix.Term);
                pp.Cbox();
                break;
            default:
                PrintTmAppTerm(pp, outer, ctx, t);
                break;
        }
    }

    private static void PrintTmAppTerm(PrettyPrinter pp, bool outer, Context ctx, ITerm t)
    {
        switch (t)
        {
            case TimesFloat timesFloat:
                pp.Write("timesfloat ");
                PrintTmTerm(pp, false, ctx, timesFloat.Left);
                pp.PrintSpace();
                PrintTmTerm(pp, false, ctx, timesFloat.Right);
                break;
            case App app:
                pp.Obox();
                PrintTmAppTerm(pp, false, ctx, app.Left);
                pp.PrintSpace();
                PrintTmTerm(pp, false, ctx, app.Right);
                pp.Cbox();                
                break;
            case TApp tapp:
                pp.Obox();
                PrintTmAppTerm(pp, false, ctx, tapp.Term);
                pp.PrintSpace();
                pp.Write("[");
                PrintType(pp, ctx, tapp.TypeArg);
                pp.Write("]");
                pp.Cbox();
                break;
            case Pred pred:
                pp.Write("pred ");
                PrintTmATerm(pp, false, ctx, pred.Of);
                break;
            case IsZero isZero:
                pp.Write("iszero ");
                PrintTmTerm(pp, false, ctx, isZero.Term);
                break;
            default:
                PrintTmPathTerm(pp, outer, ctx, t);
                break;
        }
    }

    private static void PrintTmPathTerm(PrettyPrinter pp, bool outer, Context ctx, ITerm t)
    {
        switch (t)
        {
            case Proj proj:
                PrintTmATerm(pp, false, ctx, proj.Term);
                pp.Write(".");
                pp.Write(proj.Label);
                break;
            default:
                PrintTmAscribeTerm(pp, outer, ctx, t);
                break;
        }
    }

    private static void PrintTmAscribeTerm(PrettyPrinter pp, bool outer, Context ctx, ITerm t)
    {
        switch (t)
        {
            case Ascribe ascribe:
                pp.Obox();
                PrintTmAppTerm(pp, false, ctx, ascribe.Term);
                pp.PrintSpace();
                pp.Write("as ");
                PrintType(pp, ctx, ascribe.Type);
                pp.Cbox();
                break;
            default:
                PrintTmATerm(pp, outer, ctx, t);
                break;
        }
    }

    public static void PrintType(Context ctx, IType type)
    {
        var pp = new PrettyPrinter();
        pp.PrintBreak(1, 2);
        pp.Write(": ");
        PrintType(pp, ctx, type);
        pp.NewLine();
        Console.Write(pp.ToString());
    }

    private static void PrintType(PrettyPrinter pp, Context ctx, IType type)
    {
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
                PrintType(pp, ctx, t.From);
                pp.Write(" -> ");
                PrintType(pp, ctx, t.To);
                break;
            case TypeAll all:
                var (ctxa, xa) = ctx.PickFreshName(all.Name);
                pp.Obox();
                pp.Write("All ");
                pp.Write(xa);
                pp.Write(".");
                pp.PrintSpace();
                PrintType(pp, ctxa, all.Body);
                pp.Cbox();
                break;
            case TypeSome some:
                var (ctxs, xs) = ctx.PickFreshName(some.Name);
                pp.Obox();
                pp.Write("{Some ");
                pp.Write(xs);
                pp.Write(",");
                pp.PrintSpace();
                PrintType(pp, ctxs, some.Body);
                pp.Write("}");
                pp.Cbox();
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
                            pp.Write(", ");
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
                    if (si.Item1 != (i + 1).ToString())
                    {
                        pp.Write(si.Item1);
                        pp.Write(":");
                    }
                    PrintType(pp, ctx, si.Item2);
                    if (i < source.Count - 1)
                        pp.Write(", ");
                }

                pp.Write("}");
                break;
            case TypeVar tv:
                pp.Write(ctx.Length == tv.N ? ctx.IndexToName(tv.X) : "[bad index]");
                break;
            default:
                throw new InvalidOperationException();
        }
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
