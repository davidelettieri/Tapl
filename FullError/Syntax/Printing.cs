using System;
using Common;
using FullError.Syntax.Bindings;
using FullError.Syntax.Terms;
using static FullError.Core.Typing;

namespace FullError.Syntax;

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
        PrintType(pp, ctx, type);
        pp.NewLine();
        Console.Write(pp.ToString());
    }

    private static void PrintTmATerm(PrettyPrinter pp, bool outer, Context ctx, ITerm t)
    {
        switch (t)
        {
            case True:
                pp.Write("true");
                break;
            case False:
                pp.Write("false");
                break;
            case Error:
                pp.Write("error");
                break;
            case Var var:
                pp.Write(ctx.IndexToName(var.Index));
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
            case Abs abs:
                var (ctxp, xp) = ctx.PickFreshName(abs.V);
                pp.Obox();
                pp.Write("lambda ");
                pp.Write(xp);
                pp.Write(":");
                PrintArrowType(pp, false, ctx, abs.Type);
                pp.Write(".");
                if (Small(abs.Body) && !outer)
                    pp.Break();
                else
                    pp.PrintSpace();
                PrintTmTerm(pp, outer, ctxp, abs.Body);
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
            case Try tr:
                pp.Obox0();
                pp.Write("try ");
                PrintTmTerm(pp, false, ctx, tr.Term);
                pp.PrintSpace();
                pp.Write("with ");
                PrintTmTerm(pp, false, ctx, tr.Handler);
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
            default:
                PrintTmATerm(pp, outer, ctx, t);
                break;
        }
    }

    private static void PrintType(PrettyPrinter pp, Context ctx, IType type)
        => PrintArrowType(pp, true, ctx, type);

    private static void PrintArrowType(PrettyPrinter pp, bool outer, Context ctx, IType type)
    {
        switch (type)
        {
            case TypeArrow ta:
                pp.Obox0();
                PrintAType(pp, false, ctx, ta.From);
                if (outer)
                    pp.Write(" ");
                pp.Write("->");
                if (outer)
                    pp.PrintSpace();
                else
                    pp.Break();
                PrintArrowType(pp, outer, ctx, ta.To);
                pp.Cbox();
                break;
            default:
                PrintAType(pp, outer, ctx, type);
                break;
        }
    }

    private static void PrintAType(PrettyPrinter pp, bool outer, Context ctx, IType type)
    {
        switch (type)
        {
            case TypeBot:
                pp.Write("Bot");
                break;
            case TypeTop:
                pp.Write("Top");
                break;
            case TypeBool:
                pp.Write("Bool");
                break;
            case TypeVar tv:
                pp.Write(ctx.Length == tv.N ? ctx.IndexToName(tv.X) : "[bad index]");
                break;
            default:
                pp.Write("(");
                PrintArrowType(pp, outer, ctx, type);
                pp.Write(")");
                break;
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
                PrintTmTerm(pp, true, ctx, tab.Term);
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
