using System;
using System.Globalization;
using System.Linq;
using Common;
using FullRef.Syntax.Bindings;
using FullRef.Syntax.Terms;
using static FullRef.Core.Typing;

namespace FullRef.Syntax;

public static class Printing
{
    private static bool Small(ITerm term) => term is Var;

    public static void PrintBindingType(Context ctx, IBinding binding, string name)
    {
        var pp = new PrettyPrinter();
        pp.Write(name);
        switch (binding)
        {
            case VarBind varBind:
                pp.Write(" : ");
                PrintTypeType(pp, true, ctx, varBind.Type);
                break;
            case TermAbbBind { Type: not null } termAbbBind:
                pp.Write(" : ");
                PrintTypeType(pp, true, ctx, termAbbBind.Type);
                break;
            case TermAbbBind termAbbBind:
                pp.Write(" : ");
                PrintTypeType(pp, true, ctx, TypeOf(ctx, termAbbBind.Term));
                break;
            case TypeAbbBind:
                pp.Write(" :: *");
                break;
        }

        pp.NewLine();
        Console.Write(pp.ToString());
    }

    public static void PrintTerm(Context ctx, ITerm term)
    {
        var pp = new PrettyPrinter();
        PrintTmTerm(pp, true, ctx, term);
        Console.Write(pp.ToString());
    }

    public static void PrintTmATerm(Context ctx, ITerm term)
    {
        var pp = new PrettyPrinter();
        PrintTmATerm(pp, true, ctx, term);
        Console.Write(pp.ToString());
    }

    public static void PrintTmATermWithType(Context ctx, ITerm term, IType type)
    {
        var pp = new PrettyPrinter();
        PrintTmATerm(pp, true, ctx, term);
        pp.PrintBreak(1, 2);
        pp.Write(": ");
        PrintTypeType(pp, true, ctx, type);
        pp.NewLine();
        Console.Write(pp.ToString());
    }

    public static void PrintType(Context ctx, IType type)
    {
        var pp = new PrettyPrinter();
        pp.Write(" : ");
        PrintTypeType(pp, true, ctx, type);
        pp.NewLine();
        Console.Write(pp.ToString());
    }

    public static void PrintBinding(PrettyPrinter pp, Context ctx, IBinding binding)
    {
        switch (binding)
        {
            case NameBinding:
                break;
            case VarBind varBind:
                pp.Write(" : ");
                PrintTypeType(pp, true, ctx, varBind.Type);
                break;
            case TermAbbBind termAbbBind:
                pp.Write("= ");
                PrintTmTerm(pp, true, ctx, termAbbBind.Term);
                break;
            case TypeVarBind:
                break;
            case TypeAbbBind typeAbbBind:
                pp.Write("= ");
                PrintTypeType(pp, true, ctx, typeAbbBind.Type);
                break;
        }
    }

    private static void PrintTypeType(PrettyPrinter pp, bool outer, Context ctx, IType type)
    {
        switch (type)
        {
            case TypeRef referenceType:
                pp.Write("Ref ");
                PrintTypeAType(pp, false, ctx, referenceType.Type);
                break;
            case TypeSource sourceType:
                pp.Write("Source ");
                PrintTypeAType(pp, false, ctx, sourceType.Type);
                break;
            case TypeSink sinkType:
                pp.Write("Sink ");
                PrintTypeAType(pp, false, ctx, sinkType.Type);
                break;
            default:
                PrintTypeArrow(pp, outer, ctx, type);
                break;
        }
    }

    private static void PrintTypeArrow(PrettyPrinter pp, bool outer, Context ctx, IType type)
    {
        switch (type)
        {
            case TypeArrow arrow:
                pp.Obox0();
                PrintTypeAType(pp, false, ctx, arrow.From);
                if (outer)
                    pp.Write(" ");
                pp.Write("->");
                if (outer)
                    pp.PrintSpace();
                else
                    pp.Break();
                PrintTypeArrow(pp, outer, ctx, arrow.To);
                pp.Cbox();
                break;
            default:
                PrintTypeAType(pp, outer, ctx, type);
                break;
        }
    }

    private static void PrintTypeAType(PrettyPrinter pp, bool outer, Context ctx, IType type)
    {
        switch (type)
        {
            case TypeBot:
                pp.Write("Bot");
                break;
            case TypeTop:
                pp.Write("Top");
                break;
            case TypeString:
                pp.Write("String");
                break;
            case TypeUnit:
                pp.Write("Unit");
                break;
            case TypeVariant variant:
                PrintTypeFields(pp, outer, ctx, "<", ">", variant.Variants.ToList());
                break;
            case TypeBool:
                pp.Write("Bool");
                break;
            case TypeId typeId:
                pp.Write(typeId.Name);
                break;
            case TypeRecord record:
                PrintTypeFields(pp, outer, ctx, "{", "}", record.Variants.ToList());
                break;
            case TypeFloat:
                pp.Write("Float");
                break;
            case TypeVar typeVar:
                pp.Write(ctx.Length == typeVar.N ? ctx.IndexToName(typeVar.X) : BadIndexText(ctx, typeVar.X, typeVar.N));
                break;
            case TypeNat:
                pp.Write("Nat");
                break;
            default:
                pp.Write("(");
                PrintTypeType(pp, outer, ctx, type);
                pp.Write(")");
                break;
        }
    }

    private static void PrintTypeFields(PrettyPrinter pp, bool outer, Context ctx, string open, string close, System.Collections.Generic.IReadOnlyList<(string, IType)> fields)
    {
        pp.Write(open);
        pp.OpenHvBox(0);
        for (var index = 0; index < fields.Count; index++)
        {
            var (label, type) = fields[index];
            if (label != (index + 1).ToString(CultureInfo.InvariantCulture))
            {
                pp.Write(label);
                pp.Write(":");
            }

            PrintTypeType(pp, false, ctx, type);
            if (index < fields.Count - 1)
            {
                pp.Write(",");
                if (outer)
                    pp.PrintSpace();
                else
                    pp.Break();
            }
        }

        pp.Write(close);
        pp.Cbox();
    }

    private static void PrintTmTerm(PrettyPrinter pp, bool outer, Context ctx, ITerm term)
    {
        switch (term)
        {
            case Abs abs:
            {
                var (ctx1, name) = ctx.PickFreshName(abs.V);
                pp.Obox();
                pp.Write("lambda ");
                pp.Write(name);
                pp.Write(":");
                PrintTypeType(pp, false, ctx, abs.Type);
                pp.Write(".");
                if (Small(abs.Body) && !outer)
                    pp.Break();
                else
                    pp.PrintSpace();
                PrintTmTerm(pp, outer, ctx1, abs.Body);
                pp.Cbox();
                break;
            }
            case Assign assign:
                pp.Obox();
                PrintTmAppTerm(pp, false, ctx, assign.Left);
                pp.Write(" := ");
                PrintTmAppTerm(pp, false, ctx, assign.Right);
                pp.Cbox();
                break;
            case Case @case:
                var cases = @case.Cases.ToList();
                pp.Obox();
                pp.Write("case ");
                PrintTmTerm(pp, false, ctx, @case.Term);
                pp.Write(" of");
                pp.PrintSpace();
                for (var index = 0; index < cases.Count; index++)
                {
                    var (label, variable, body) = cases[index];
                    var (ctx1, name) = ctx.PickFreshName(variable);
                    pp.Write("<");
                    pp.Write(label);
                    pp.Write("=");
                    pp.Write(name);
                    pp.Write(">==>");
                    PrintTmTerm(pp, false, ctx1, body);
                    if (index < cases.Count - 1)
                    {
                        pp.PrintSpace();
                        pp.Write("| ");
                    }
                }
                pp.Cbox();
                break;
            case Let letTerm:
                pp.Obox0();
                pp.Write("let ");
                pp.Write(letTerm.Variable);
                pp.Write(" = ");
                PrintTmTerm(pp, false, ctx, letTerm.LetTerm);
                pp.PrintSpace();
                pp.Write("in");
                pp.PrintSpace();
                PrintTmTerm(pp, false, ctx.AddName(letTerm.Variable), letTerm.InTerm);
                pp.Cbox();
                break;
            case Fix fix:
                pp.Obox();
                pp.Write("fix ");
                PrintTmTerm(pp, false, ctx, fix.Term);
                pp.Cbox();
                break;
            case If ifTerm:
                pp.Obox0();
                pp.Write("if ");
                PrintTmTerm(pp, false, ctx, ifTerm.Condition);
                pp.PrintSpace();
                pp.Write("then ");
                PrintTmTerm(pp, false, ctx, ifTerm.Then);
                pp.PrintSpace();
                pp.Write("else ");
                PrintTmTerm(pp, false, ctx, ifTerm.Else);
                pp.Cbox();
                break;
            default:
                PrintTmAppTerm(pp, outer, ctx, term);
                break;
        }
    }

    private static void PrintTmAppTerm(PrettyPrinter pp, bool outer, Context ctx, ITerm term)
    {
        switch (term)
        {
            case App app:
                pp.Obox0();
                PrintTmAppTerm(pp, false, ctx, app.Left);
                pp.PrintSpace();
                PrintTmATerm(pp, false, ctx, app.Right);
                pp.Cbox();
                break;
            case Ref reference:
                pp.Obox();
                pp.Write("ref ");
                PrintTmATerm(pp, false, ctx, reference.Term);
                pp.Cbox();
                break;
            case Deref deref:
                pp.Obox();
                pp.Write("!");
                PrintTmATerm(pp, false, ctx, deref.Term);
                pp.Cbox();
                break;
            case TimesFloat timesFloat:
                pp.Write("timesfloat ");
                PrintTmATerm(pp, false, ctx, timesFloat.Left);
                pp.Write(" ");
                PrintTmATerm(pp, false, ctx, timesFloat.Right);
                break;
            case Pred pred:
                pp.Write("pred ");
                PrintTmATerm(pp, false, ctx, pred.Of);
                break;
            case IsZero isZero:
                pp.Write("iszero ");
                PrintTmATerm(pp, false, ctx, isZero.Term);
                break;
            default:
                PrintTmPathTerm(pp, outer, ctx, term);
                break;
        }
    }

    private static void PrintTmAscribeTerm(PrettyPrinter pp, bool outer, Context ctx, ITerm term)
    {
        switch (term)
        {
            case Ascribe ascribe:
                pp.Obox0();
                PrintTmAppTerm(pp, false, ctx, ascribe.Term);
                pp.PrintSpace();
                pp.Write("as ");
                PrintTypeType(pp, false, ctx, ascribe.Type);
                pp.Cbox();
                break;
            default:
                PrintTmATerm(pp, outer, ctx, term);
                break;
        }
    }

    private static void PrintTmPathTerm(PrettyPrinter pp, bool outer, Context ctx, ITerm term)
    {
        switch (term)
        {
            case Proj proj:
                PrintTmATerm(pp, false, ctx, proj.Term);
                pp.Write(".");
                pp.Write(proj.Label);
                break;
            default:
                PrintTmAscribeTerm(pp, outer, ctx, term);
                break;
        }
    }

    private static void PrintTmATerm(PrettyPrinter pp, bool outer, Context ctx, ITerm term)
    {
        switch (term)
        {
            case Var variable:
                pp.Write(ctx.Length == variable.ContextLength
                    ? ctx.IndexToName(variable.Index)
                    : BadIndexText(ctx, variable.Index, variable.ContextLength));
                break;
            case StringTerm stringTerm:
                pp.Write("\"");
                pp.Write(stringTerm.Value);
                pp.Write("\"");
                break;
            case Unit:
                pp.Write("unit");
                break;
            case Loc loc:
                pp.Write("<loc #");
                pp.Write(loc.Location.ToString(CultureInfo.InvariantCulture));
                pp.Write(">");
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
                PrintTypeType(pp, outer, ctx, tag.Type);
                pp.Cbox();
                break;
            case True:
                pp.Write("true");
                break;
            case False:
                pp.Write("false");
                break;
            case Float floating:
                pp.Write(floating.Value.ToString(CultureInfo.InvariantCulture));
                break;
            case Record record:
                pp.Write("{");
                pp.OpenHvBox(0);
                for (var index = 0; index < record.Fields.Count; index++)
                {
                    var (label, fieldTerm) = record.Fields[index];
                    if (label != (index + 1).ToString(CultureInfo.InvariantCulture))
                    {
                        pp.Write(label);
                        pp.Write("=");
                    }

                    PrintTmTerm(pp, false, ctx, fieldTerm);
                    if (index < record.Fields.Count - 1)
                    {
                        pp.Write(",");
                        if (outer)
                            pp.PrintSpace();
                        else
                            pp.Break();
                    }
                }
                pp.Write("}");
                pp.Cbox();
                break;
            case Zero:
                pp.Write("0");
                break;
            case Succ succ:
                PrintNat(pp, ctx, succ.Of, 1);
                break;
            case Inert inert:
                pp.Write("inert[");
                PrintTypeType(pp, false, ctx, inert.Type);
                pp.Write("]");
                break;
            default:
                pp.Write("(");
                PrintTmTerm(pp, outer, ctx, term);
                pp.Write(")");
                break;
        }
    }

    private static void PrintNat(PrettyPrinter pp, Context ctx, ITerm term, int value)
    {
        switch (term)
        {
            case Zero:
                pp.Write(value.ToString(CultureInfo.InvariantCulture));
                break;
            case Succ succ:
                PrintNat(pp, ctx, succ.Of, value + 1);
                break;
            default:
                pp.Write("(succ ");
                PrintTmATerm(pp, false, ctx, term);
                pp.Write(")");
                break;
        }
    }

    private static string BadIndexText(Context ctx, int index, int length)
    {
        var names = string.Join(" ", ctx.Value.Select(entry => entry.Item1));
        return $"[bad index: {index}/{length} in {{{(names.Length == 0 ? string.Empty : " " + names)} }}]";
    }
}
