using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Common;
using FullUpdate.Core;
using FullUpdate.Syntax;
using FullUpdate.Syntax.Bindings;
using FullUpdate.Syntax.Terms;

namespace FullUpdate.Syntax;

public static class Printing
{
    private static bool Small(ITerm t) => t is Var;

    // ---- Public entry points ----

    public static void PrintTmATerm(Context ctx, ITerm t)
    {
        var pp = new PrettyPrinter();
        PrintTmATerm(pp, true, ctx, t);
        Console.Write(pp.ToString());
    }

    public static void PrintType(Context ctx, IType type)
    {
        var pp = new PrettyPrinter();
        pp.PrintBreak(1, 2);
        pp.Write(": ");
        PrintType(pp, true, ctx, type);
        pp.NewLine();
        Console.Write(pp.ToString());
    }

    public static void PrintBindingType(Context ctx, IBinding b, string name)
    {
        var pp = new PrettyPrinter();
        pp.Write(name);
        pp.Write(" ");
        PrintBindingType(pp, ctx, b);
        pp.NewLine();
        Console.Write(pp.ToString());
    }

    public static void PrintBodyType(PrettyPrinter pp, Context ctx, IType t)
    {
        PrintType(pp, true, ctx, t);
    }

    // ---- Kind printing ----

    private static void PrintKind(PrettyPrinter pp, bool outer, IKind k)
    {
        switch (k)
        {
            case KnArr kArr:
                pp.Obox0();
                PrintAKind(pp, false, kArr.From);
                if (outer) pp.Write(" "); else pp.PrintBreak(0, 0);
                pp.Write("=>");
                if (outer) pp.PrintSpace(); else pp.PrintBreak(0, 0);
                PrintKind(pp, outer, kArr.To);
                pp.Cbox();
                break;
            default:
                PrintAKind(pp, outer, k);
                break;
        }
    }

    private static void PrintAKind(PrettyPrinter pp, bool outer, IKind k)
    {
        switch (k)
        {
            case KnStar: pp.Write("*"); break;
            default:
                pp.Write("(");
                PrintKind(pp, outer, k);
                pp.Write(")");
                break;
        }
    }

    private static void ProKind(PrettyPrinter pp, IKind k)
    {
        if (k is not KnStar)
        {
            pp.Write("::");
            PrintKind(pp, false, k);
        }
    }

    // ---- Bound printing ----

    /// <summary>
    /// If <paramref name="t"/> is MakeTop(K) for some non-* kind K, returns that K.
    /// MakeTop(KnStar) = TypeTop; MakeTop(KnArr(k1,k2)) = TypeAbs("_",k1,MakeTop(k2)).
    /// </summary>
    private static IKind? TryGetMakeTopKind(IType t) => t switch
    {
        TypeTop => new KnStar(),
        TypeAbs { Body: var body } abs when TryGetMakeTopKind(body) is KnStar =>
            new KnArr(abs.Kind, new KnStar()),
        TypeAbs { Body: var body } abs when TryGetMakeTopKind(body) is KnArr karr =>
            new KnArr(abs.Kind, karr),
        _ => null
    };

    // proty: prints "<:T" if T != Top, or "::K" if T is MakeTop(K) for non-* K
    private static void ProType(PrettyPrinter pp, Context ctx, IType t)
    {
        if (t is TypeTop) return;
        var k = TryGetMakeTopKind(t);
        if (k is KnArr)
        {
            // higher-kinded variable: print ::K
            pp.Write("::");
            PrintKind(pp, false, k);
        }
        else
        {
            pp.Write("<:");
            PrintType(pp, false, ctx, t);
        }
    }

    // ---- Type printing ----

    private static void PrintType(PrettyPrinter pp, bool outer, Context ctx, IType t)
    {
        switch (t)
        {
            case TypeAbs abs:
                var (ctx1, x1) = ctx.PickFreshName(abs.Name);
                pp.Obox();
                pp.Write("lambda ");
                pp.Write(x1);
                ProKind(pp, abs.Kind);
                pp.Write(".");
                if (outer) pp.PrintSpace(); else pp.PrintBreak(0, 0);
                PrintType(pp, outer, ctx1, abs.Body);
                pp.Cbox();
                break;
            case TypeAll all:
                var (ctx2, x2) = ctx.PickFreshName(all.Name);
                pp.Obox();
                pp.Write("All ");
                pp.Write(x2);
                ProType(pp, ctx, all.Bound);
                pp.Write(".");
                pp.PrintSpace();
                PrintType(pp, outer, ctx2, all.Body);
                pp.Cbox();
                break;
            default:
                PrintArrowType(pp, outer, ctx, t);
                break;
        }
    }

    private static void PrintArrowType(PrettyPrinter pp, bool outer, Context ctx, IType t)
    {
        switch (t)
        {
            case TypeArrow arr:
                pp.Obox0();
                PrintAppType(pp, false, ctx, arr.From);
                if (outer) pp.Write(" "); else pp.PrintBreak(0, 0);
                pp.Write("->");
                if (outer) pp.PrintSpace(); else pp.PrintBreak(0, 0);
                PrintArrowType(pp, outer, ctx, arr.To);
                pp.Cbox();
                break;
            default:
                PrintAppType(pp, outer, ctx, t);
                break;
        }
    }

    private static void PrintAppType(PrettyPrinter pp, bool outer, Context ctx, IType t)
    {
        switch (t)
        {
            case TypeApp app:
                pp.Obox0();
                PrintAppType(pp, false, ctx, app.T1);
                pp.PrintSpace();
                PrintAType(pp, false, ctx, app.T2);
                pp.Cbox();
                break;
            default:
                PrintAType(pp, outer, ctx, t);
                break;
        }
    }

    private static void PrintAType(PrettyPrinter pp, bool outer, Context ctx, IType t)
    {
        switch (t)
        {
            case TypeVar tv:
                pp.Write(ctx.Length == tv.N ? ctx.IndexToName(tv.X) : $"[bad index: {tv.X}/{tv.N}]");
                break;
            case TypeString: pp.Write("String"); break;
            case TypeSome some:
                var (ctx3, x3) = ctx.PickFreshName(some.Name);
                pp.Obox();
                pp.Write("{Some ");
                pp.Write(x3);
                ProType(pp, ctx, some.Bound);
                pp.Write(",");
                if (outer) pp.PrintSpace(); else pp.PrintBreak(0, 0);
                PrintType(pp, false, ctx3, some.Body);
                pp.Write("}");
                pp.Cbox();
                break;
            case TypeRecord rec:
                PrintTypeRecord(pp, outer, ctx, rec);
                break;
            case TypeBool: pp.Write("Bool"); break;
            case TypeNat: pp.Write("Nat"); break;
            case TypeUnit: pp.Write("Unit"); break;
            case TypeTop: pp.Write("Top"); break;
            case TypeId id: pp.Write(id.Name); break;
            case TypeFloat: pp.Write("Float"); break;
            default:
                pp.Write("(");
                PrintType(pp, outer, ctx, t);
                pp.Write(")");
                break;
        }
    }

    private static void PrintTypeRecord(PrettyPrinter pp, bool outer, Context ctx, TypeRecord rec)
    {
        pp.Write("{");
        pp.OpenHvBox(0);
        var fields = rec.Fields.ToList();
        for (int i = 0; i < fields.Count; i++)
        {
            var (label, variance, type) = fields[i];
            if (variance == Variance.Invariant) pp.Write("#");
            if (label != (i + 1).ToString())
            {
                pp.Write(label);
                pp.Write(":");
            }
            PrintType(pp, false, ctx, type);
            if (i < fields.Count - 1)
            {
                pp.Write(",");
                pp.PrintSpace();
            }
        }
        pp.Write("}");
        pp.Cbox();
    }

    // ---- Binding type printing ----

    private static void PrintBindingType(PrettyPrinter pp, Context ctx, IBinding b)
    {
        switch (b)
        {
            case NameBinding: break;
            case VarBind vb: pp.Write(": "); PrintType(pp, true, ctx, vb.Type); break;
            case TypeVarBind tvb:
                var tvbKind = TryGetMakeTopKind(tvb.Bound);
                if (tvb.Bound is TypeTop)
                    break; // default, no annotation needed
                else if (tvbKind is KnArr)
                {
                    pp.Write(":: ");
                    PrintKind(pp, true, tvbKind);
                }
                else
                {
                    pp.Write("<: ");
                    PrintType(pp, false, ctx, tvb.Bound);
                }
                break;
            case TypeAbbBind tab:
                pp.Write(":: ");
                if (tab.Kind is not null) PrintKind(pp, true, tab.Kind);
                else PrintKind(pp, true, Typing.KindOf(ctx, tab.Type));
                break;
            case TermAbbBind tab:
                pp.Write(": ");
                if (tab.Type is not null) PrintType(pp, true, ctx, tab.Type);
                else PrintType(pp, true, ctx, Typing.TypeOf(ctx, tab.Term));
                break;
        }
    }

    // ---- Term printing ----

    private static void PrintTmTerm(PrettyPrinter pp, bool outer, Context ctx, ITerm t)
    {
        switch (t)
        {
            case Abs abs:
                var (ctx1, x1) = ctx.PickFreshName(abs.V);
                pp.Obox();
                pp.Write("lambda ");
                pp.Write(x1);
                pp.Write(":");
                PrintType(pp, false, ctx, abs.Type);
                pp.Write(".");
                if (Small(abs.Body) && !outer) pp.PrintBreak(0, 0);
                else pp.PrintSpace();
                PrintTmTerm(pp, outer, ctx1, abs.Body);
                pp.Cbox();
                break;

            case TAbs tabs:
                var (ctx2, x2) = ctx.PickFreshName(tabs.TypeVar);
                pp.Obox();
                pp.Write("lambda ");
                pp.Write(x2);
                ProType(pp, ctx, tabs.Bound);
                pp.Write(".");
                if (Small(tabs.Body) && !outer) pp.PrintBreak(0, 0);
                else pp.PrintSpace();
                PrintTmTerm(pp, outer, ctx2, tabs.Body);
                pp.Cbox();
                break;

            case Unpack unpack:
                var (ctx3, tyX3) = ctx.PickFreshName(unpack.TypeVar);
                var (ctx4, x4) = ctx3.PickFreshName(unpack.V);
                pp.Obox();
                pp.Write("let {");
                pp.Write(tyX3);
                pp.Write(",");
                pp.Write(x4);
                pp.Write("} =");
                pp.PrintSpace();
                PrintTmTerm(pp, false, ctx, unpack.Package);
                pp.Write(" in ");
                PrintTmTerm(pp, outer, ctx4, unpack.Body);
                pp.Cbox();
                break;

            case If ift:
                pp.Obox0();
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

            case Update upd:
                pp.Obox();
                PrintTmAppTerm(pp, outer, ctx, upd.Record);
                pp.PrintSpace();
                pp.Write("<-");
                pp.Write(" ");
                pp.Write(upd.Label);
                pp.Write(" = ");
                PrintTmTerm(pp, outer, ctx, upd.NewValue);
                pp.Cbox();
                break;

            case Let let:
                pp.Obox0();
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
            case App app:
                pp.Obox0();
                PrintTmAppTerm(pp, false, ctx, app.Left);
                pp.PrintSpace();
                PrintTmATerm(pp, false, ctx, app.Right);
                pp.Cbox();
                break;
            case TApp tapp:
                pp.Obox0();
                PrintTmAppTerm(pp, false, ctx, tapp.Term);
                pp.PrintSpace();
                pp.Write("[");
                PrintType(pp, false, ctx, tapp.TypeArg);
                pp.Write("]");
                pp.Cbox();
                break;
            case Pred pred:
                pp.Write("pred ");
                PrintTmATerm(pp, false, ctx, pred.Of);
                break;
            case IsZero iz:
                pp.Write("iszero ");
                PrintTmATerm(pp, false, ctx, iz.Term);
                break;
            case TimesFloat tf:
                pp.Write("timesfloat ");
                PrintTmATerm(pp, false, ctx, tf.Left);
                pp.Write(" ");
                PrintTmATerm(pp, false, ctx, tf.Right);
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
            case Ascribe asc:
                pp.Obox0();
                PrintTmAppTerm(pp, false, ctx, asc.Term);
                pp.PrintSpace();
                pp.Write("as ");
                PrintType(pp, false, ctx, asc.Type);
                pp.Cbox();
                break;
            default:
                PrintTmATerm(pp, outer, ctx, t);
                break;
        }
    }

    private static void PrintTmATerm(PrettyPrinter pp, bool outer, Context ctx, ITerm t)
    {
        switch (t)
        {
            case Var var:
                pp.Write(ctx.IndexToName(var.Index));
                break;
            case StringTerm s:
                pp.Write("\"");
                pp.Write(s.Value);
                pp.Write("\"");
                break;
            case Pack pack:
                pp.Obox();
                pp.Write("{*");
                PrintType(pp, false, ctx, pack.WitnessType);
                pp.Write(",");
                if (outer) pp.PrintSpace(); else pp.PrintBreak(0, 0);
                PrintTmTerm(pp, false, ctx, pack.Term);
                pp.Write("}");
                pp.PrintSpace();
                pp.Write("as ");
                PrintType(pp, outer, ctx, pack.ExistType);
                pp.Cbox();
                break;
            case Record rec:
                PrintTermRecord(pp, outer, ctx, rec);
                break;
            case True: pp.Write("true"); break;
            case False: pp.Write("false"); break;
            case Zero: pp.Write("0"); break;
            case Succ succ:
                PrintNat(pp, ctx, succ.Of, 1);
                break;
            case Unit: pp.Write("unit"); break;
            case Float f:
                var fstr = f.Value.ToString("G", CultureInfo.InvariantCulture);
                // OCaml adds trailing dot for integer-valued floats (e.g. 2.)
                if (!fstr.Contains('.') && !fstr.Contains('E') && !fstr.Contains('e'))
                    fstr += ".";
                pp.Write(fstr);
                break;
            case Inert i:
                pp.Write("inert[");
                PrintType(pp, false, ctx, i.Type);
                pp.Write("]");
                break;
            default:
                pp.Write("(");
                PrintTmTerm(pp, outer, ctx, t);
                pp.Write(")");
                break;
        }
    }

    private static void PrintNat(PrettyPrinter pp, Context ctx, ITerm t, int value)
    {
        switch (t)
        {
            case Zero: pp.Write(value.ToString()); break;
            case Succ succ: PrintNat(pp, ctx, succ.Of, value + 1); break;
            default:
                pp.Write("(succ ");
                PrintTmATerm(pp, false, ctx, t);
                pp.Write(")");
                break;
        }
    }

    private static void PrintTermRecord(PrettyPrinter pp, bool outer, Context ctx, Record rec)
    {
        pp.Write("{");
        pp.OpenHvBox(0);
        var fields = rec.Fields;
        for (int i = 0; i < fields.Count; i++)
        {
            var (label, variance, term) = fields[i];
            if (variance == Variance.Invariant) pp.Write("#");
            if (label != (i + 1).ToString())
            {
                pp.Write(label);
                pp.Write("=");
            }
            PrintTmTerm(pp, false, ctx, term);
            if (i < fields.Count - 1)
            {
                pp.Write(",");
                pp.PrintSpace();
            }
        }
        pp.Write("}");
        pp.Cbox();
    }
}
