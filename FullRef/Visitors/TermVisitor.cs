using System;
using System.Linq;
using Common;
using FullRef.Syntax;
using FullRef.Syntax.Terms;

namespace FullRef.Visitors;

public sealed class TermVisitor : FullRefBaseVisitor<Func<Context, ITerm>>
{
    private readonly CasesVisitor _casesVisitor;
    private readonly TypeVisitor _typeVisitor = new();
    private readonly FieldsVisitor _fieldsVisitor;

    public TermVisitor()
    {
        _fieldsVisitor = new FieldsVisitor(this);
        _casesVisitor = new CasesVisitor(this);
    }

    public override Func<Context, ITerm> VisitTerm_appterm(FullRefParser.Term_apptermContext context) => Visit(context.appterm());

    public override Func<Context, ITerm> VisitTerm_assign(FullRefParser.Term_assignContext context)
    {
        var token = context.COLONEQ().Symbol;
        var info = new Common.FileInfo(context.GetText(), token.Line, token.Column);
        var appTerms = context.appterm();
        var left = Visit(appTerms[0]);
        var right = Visit(appTerms[1]);
        return ctx => new Assign(info, left(ctx), right(ctx));
    }

    public override Func<Context, ITerm> VisitTerm_ift(FullRefParser.Term_iftContext context)
    {
        var terms = context.term();
        var condition = Visit(terms[0]);
        var then = Visit(terms[1]);
        var @else = Visit(terms[2]);
        var info = context.GetFileInfo();
        return c => new If(info, condition(c), then(c), @else(c));
    }

    public override Func<Context, ITerm> VisitTerm_caseOf(FullRefParser.Term_caseOfContext context)
    {
        var info = context.GetFileInfo();
        var term = Visit(context.term());
        var @case = _casesVisitor.Visit(context.cases());
        return ctx => new Case(info, term(ctx), @case(ctx));
    }

    public override Func<Context, ITerm> VisitTerm_llcid(FullRefParser.Term_llcidContext context)
    {
        var v = context.LCID().GetText();
        var info = context.GetFileInfo();
        var type = _typeVisitor.Visit(context.type());
        var term = Visit(context.term());
        return ctx =>
        {
            var ctx1 = ctx.AddName(v);
            return new Abs(info, v, type(ctx), term(ctx1));
        };
    }

    public override Func<Context, ITerm> VisitTerm_luc(FullRefParser.Term_lucContext context)
    {
        var info = context.GetFileInfo();
        var type = _typeVisitor.Visit(context.type());
        var term = Visit(context.term());
        return ctx =>
        {
            var ctx1 = ctx.AddName("_");
            return new Abs(info, "_", type(ctx), term(ctx1));
        };
    }

    public override Func<Context, ITerm> VisitTerm_ll(FullRefParser.Term_llContext context)
    {
        var info = context.GetFileInfo();
        var v = context.LCID().GetText();
        var terms = context.term();
        var term0 = Visit(terms[0]);
        var term1 = Visit(terms[1]);

        return ctx => new Let(info, v, term0(ctx), term1(ctx.AddName(v)));
    }

    public override Func<Context, ITerm> VisitTerm_lu(FullRefParser.Term_luContext context)
    {
        var info = context.GetFileInfo();
        var terms = context.term();
        var term0 = Visit(terms[0]);
        var term1 = Visit(terms[1]);

        return ctx => new Let(info, "_", term0(ctx), term1(ctx.AddName("_")));
    }

    public override Func<Context, ITerm> VisitTerm_letrec(FullRefParser.Term_letrecContext context)
    {
        var info = context.GetFileInfo();
        var v = context.LCID().GetText();
        var type = _typeVisitor.Visit(context.type());
        var terms = context.term();
        var eqTerm = Visit(terms[0]);
        var inTerm = Visit(terms[1]);

        return ctx =>
        {
            var abs = new Abs(info, v, type(ctx), eqTerm(ctx.AddName(v)));
            var fix = new Fix(info, abs);
            return new Let(info, v, fix, inTerm(ctx.AddName(v)));
        };
    }

    public override Func<Context, ITerm> VisitAppterm_times(FullRefParser.Appterm_timesContext context)
    {
        var info = context.GetFileInfo();
        var pathTerms = context.pathterm();
        var p0 = Visit(pathTerms[0]);
        var p1 = Visit(pathTerms[1]);

        return ctx => new TimesFloat(info, p0(ctx), p1(ctx));
    }

    public override Func<Context, ITerm> VisitAppterm_iszero(FullRefParser.Appterm_iszeroContext context)
    {
        var info = context.GetFileInfo();
        var pathTerm = Visit(context.pathterm());

        return ctx => new IsZero(info, pathTerm(ctx));
    }

    public override Func<Context, ITerm> VisitAppterm_path(FullRefParser.Appterm_pathContext context) => Visit(context.pathterm());

    public override Func<Context, ITerm> VisitAppterm_ref(FullRefParser.Appterm_refContext context)
    {
        var info = context.GetFileInfo();
        var pathTerm = Visit(context.pathterm());
        return ctx => new Ref(info, pathTerm(ctx));
    }

    public override Func<Context, ITerm> VisitAppterm_deref(FullRefParser.Appterm_derefContext context)
    {
        var info = context.GetFileInfo();
        var pathTerm = Visit(context.pathterm());
        return ctx => new Deref(info, pathTerm(ctx));
    }

    public override Func<Context, ITerm> VisitAppterm_app_path(FullRefParser.Appterm_app_pathContext context)
    {
        var info = context.GetFileInfo();
        var a = Visit(context.appterm());
        var p = Visit(context.pathterm());

        return ctx => new App(info, a(ctx), p(ctx));
    }

    public override Func<Context, ITerm> VisitAppterm_succ(FullRefParser.Appterm_succContext context)
    {
        var info = context.GetFileInfo();
        var pathTerm = Visit(context.pathterm());

        return ctx => new Succ(info, pathTerm(ctx));
    }

    public override Func<Context, ITerm> VisitAppterm_pred(FullRefParser.Appterm_predContext context)
    {
        var info = context.GetFileInfo();
        var pathTerm = Visit(context.pathterm());

        return ctx => new Pred(info, pathTerm(ctx));
    }

    public override Func<Context, ITerm> VisitAppterm_fix(FullRefParser.Appterm_fixContext context)
    {
        var info = context.GetFileInfo();
        var pathTerm = Visit(context.pathterm());

        return ctx => new Fix(info, pathTerm(ctx));
    }

    public override Func<Context, ITerm> VisitAscribeterm_aaa(FullRefParser.Ascribeterm_aaaContext context)
    {
        var info = context.GetFileInfo();
        var at = Visit(context.aterm());
        var type = _typeVisitor.Visit(context.type());
        return ctx => new Ascribe(info, at(ctx), type(ctx));
    }

    public override Func<Context, ITerm> VisitAscribeterm_a(FullRefParser.Ascribeterm_aContext context) => Visit(context.aterm());

    public override Func<Context, ITerm> VisitPathterm_intv(FullRefParser.Pathterm_intvContext context)
    {
        var info = context.GetFileInfo();
        var pt = Visit(context.pathterm());
        var intv = context.INTV().GetText();
        return c => new Proj(info, pt(c), intv);
    }

    public override Func<Context, ITerm> VisitPathterm_lcid(FullRefParser.Pathterm_lcidContext context)
    {
        var info = context.GetFileInfo();
        var pt = Visit(context.pathterm());
        var lcid = context.LCID().GetText();
        return c => new Proj(info, pt(c), lcid);
    }

    public override Func<Context, ITerm> VisitPathterm_asterm(FullRefParser.Pathterm_astermContext context) => Visit(context.ascribeterm());

    public override Func<Context, ITerm> VisitTermseq_term(FullRefParser.Termseq_termContext context) => Visit(context.term());

    public override Func<Context, ITerm> VisitTermseq_termseq(FullRefParser.Termseq_termseqContext context)
    {
        var info = context.GetFileInfo();
        var t = Visit(context.term());
        var ts = Visit(context.termseq());

        return c => new App(info, new Abs(info, "_", new TypeUnit(), ts(c.AddName("_"))), t(c));
    }

    public override Func<Context, ITerm> VisitAterm_paren(FullRefParser.Aterm_parenContext context) => Visit(context.termseq());

    public override Func<Context, ITerm> VisitAterm_inert(FullRefParser.Aterm_inertContext context)
    {
        var info = context.GetFileInfo();
        var type = _typeVisitor.Visit(context.type());
        return c => new Inert(info, type(c));
    }

    public override Func<Context, ITerm> VisitAterm_true(FullRefParser.Aterm_trueContext context)
    {
        var info = context.GetFileInfo();
        return _ => new True(info);
    }

    public override Func<Context, ITerm> VisitAterm_false(FullRefParser.Aterm_falseContext context)
    {
        var info = context.GetFileInfo();
        return _ => new False(info);
    }

    public override Func<Context, ITerm> VisitAterm_lt(FullRefParser.Aterm_ltContext context)
    {
        var info = context.GetFileInfo();
        var v = context.LCID().GetText();
        var term = Visit(context.term());
        var type = _typeVisitor.Visit(context.type());
        return c => new Tag(info, v, term(c), type(c));
    }

    public override Func<Context, ITerm> VisitAterm_lcid(FullRefParser.Aterm_lcidContext context)
    {
        var info = context.GetFileInfo();
        var v = context.LCID().GetText();

        return c => new Var(info, c.NameToIndex(v), c.Length);
    }

    public override Func<Context, ITerm> VisitAterm_stringv(FullRefParser.Aterm_stringvContext context)
    {
        var info = context.GetFileInfo();
        var v = context.STRINGV().GetText().Trim('"');
        return _ => new StringTerm(v);
    }

    public override Func<Context, ITerm> VisitAterm_unit(FullRefParser.Aterm_unitContext context)
    {
        var info = context.GetFileInfo();
        return _ => new Unit(info);
    }

    public override Func<Context, ITerm> VisitAterm_fields(FullRefParser.Aterm_fieldsContext context)
    {
        var info = context.GetFileInfo();
        var fields = _fieldsVisitor.Visit(context.fields());

        return c => new Record(info, fields((c, 1)).ToList());
    }

    public override Func<Context, ITerm> VisitAterm_floatv(FullRefParser.Aterm_floatvContext context)
    {
        var info = context.GetFileInfo();
        var value = double.Parse(context.FLOATV().GetText(), System.Globalization.CultureInfo.InvariantCulture);
        return _ => new Float(info, value);
    }

    public override Func<Context, ITerm> VisitAterm_intv(FullRefParser.Aterm_intvContext context)
    {
        var info = context.GetFileInfo();
        var value = int.Parse(context.INTV().GetText(), System.Globalization.CultureInfo.InvariantCulture);
        return _ => f(value);

        ITerm f(int n) => n == 0 ? new Zero(info) : new Succ(info, f(n - 1));
    }
}