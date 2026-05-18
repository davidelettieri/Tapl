using System;
using Common;
using FullRecon.Syntax.Bindings;
using FullRecon.Syntax.Terms;

namespace FullRecon.Syntax;

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
                PrintType(pp, true, ctx, vb.Type);
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

    public static void PrintTmATermWithType(Context ctx, ITerm t, IType type)
    {
        var termPp = new PrettyPrinter(67);
        PrintTmATerm(termPp, true, ctx, t);
        var termStr = termPp.ToString();

        var typePp = new PrettyPrinter(67);
        PrintType(typePp, true, ctx, type);
        var typeStr = typePp.ToString();

        // " : " = 3 chars; wrap if combined line reaches or exceeds OCaml margin 67
        if (termStr.Length + 3 + typeStr.Length < 67)
        {
            Console.Write(termStr);
            Console.Write(" : ");
            Console.Write(typeStr);
            Console.WriteLine();
        }
        else
        {
            Console.Write(termStr);
            Console.WriteLine();
            Console.Write("  : ");
            Console.Write(typeStr);
            Console.WriteLine();
        }
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
            case Var var:
                pp.Write(ctx.IndexToName(var.Index));
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

    private static void PrintNat(PrettyPrinter pp, Context ctx, ITerm t, int value)
    {
        switch (t)
        {
            case Zero:
                pp.Write(value.ToString());
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
            case Abs abs when abs.Type is not null:
            {
                var (ctxp, xp) = ctx.PickFreshName(abs.V);
                pp.Obox();
                pp.Write("lambda ");
                pp.Write(xp);
                pp.Write(":");
                PrintType(pp, false, ctx, abs.Type);
                pp.Write(".");
                if (Small(abs.Body) && !outer)
                    pp.Break();
                else
                    pp.PrintSpace();
                PrintTmTerm(pp, outer, ctxp, abs.Body);
                pp.Cbox();
                break;
            }
            case Abs abs:
            {
                var (ctxp, xp) = ctx.PickFreshName(abs.V);
                pp.Obox();
                pp.Write("lambda ");
                pp.Write(xp);
                pp.Write(".");
                if (Small(abs.Body) && !outer)
                    pp.Break();
                else
                    pp.PrintSpace();
                PrintTmTerm(pp, outer, ctxp, abs.Body);
                pp.Cbox();
                break;
            }
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
            case Pred pred:
                pp.Write("pred ");
                PrintTmATerm(pp, false, ctx, pred.Of);
                break;
            case IsZero isZero:
                pp.Write("iszero ");
                PrintTmATerm(pp, false, ctx, isZero.Term);
                break;
            default:
                PrintTmATerm(pp, outer, ctx, t);
                break;
        }
    }

    private static void PrintType(PrettyPrinter pp, bool outer, Context ctx, IType type)
        => PrintArrowType(pp, outer, ctx, type);

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
            case TypeBool:
                pp.Write("Bool");
                break;
            case TypeNat:
                pp.Write("Nat");
                break;
            case TypeId ti:
                pp.Write(ti.Name);
                break;
            default:
                pp.Write("(");
                PrintArrowType(pp, outer, ctx, type);
                pp.Write(")");
                break;
        }
    }
}
