using System;
using System.Linq;
using Common;
using FullPoly.Syntax;
using FullPoly.Syntax.Terms;

namespace FullPoly.Visitors;

public sealed class TermVisitor : FullPolyBaseVisitor<Func<Context, ITerm>>
{
    private readonly CasesVisitor _casesVisitor;
    private readonly TypeVisitor _typeVisitor = new();
    private readonly FieldsVisitor _fieldsVisitor;

    public TermVisitor()
    {
        _fieldsVisitor = new FieldsVisitor(this);
        _casesVisitor = new CasesVisitor(this);
    }

    public override Func<Context, ITerm> VisitTerm_appterm(FullPolyParser.Term_apptermContext context) => Visit(context.appterm());

    public override Func<Context, ITerm> VisitTerm_ift(FullPolyParser.Term_iftContext context)
    {
        var terms = context.term();
        var condition = Visit(terms[0]);
        var then = Visit(terms[1]);
        var @else = Visit(terms[2]);
        var info = context.GetFileInfo();
        return c => new If(info, condition(c), then(c), @else(c));
    }

    public override Func<Context, ITerm> VisitTerm_caseOf(FullPolyParser.Term_caseOfContext context)
    {
        var info = context.GetFileInfo();
        var term = Visit(context.term());
        var @case = _casesVisitor.Visit(context.cases());
        return ctx => new Case(info, term(ctx), @case(ctx));
    }

    public override Func<Context, ITerm> VisitTerm_llcid(FullPolyParser.Term_llcidContext context)
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

    public override Func<Context, ITerm> VisitTerm_luc(FullPolyParser.Term_lucContext context)
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

    public override Func<Context, ITerm> VisitTerm_tlambda(FullPolyParser.Term_tlambdaContext context)
    {
        var info = context.GetFileInfo();
        var name = context.UCID().GetText();
        var body = Visit(context.term());
        return ctx => new TAbs(info, name, body(ctx.AddName(name)));
    }

    public override Func<Context, ITerm> VisitTerm_ll(FullPolyParser.Term_llContext context)
    {
        var info = context.GetFileInfo();
        var v = context.LCID().GetText();
        var terms = context.term();
        var term0 = Visit(terms[0]);
        var term1 = Visit(terms[1]);

        return ctx => new Let(info, v, term0(ctx), term1(ctx.AddName(v)));
    }

    public override Func<Context, ITerm> VisitTerm_lu(FullPolyParser.Term_luContext context)
    {
        var info = context.GetFileInfo();
        var terms = context.term();
        var term0 = Visit(terms[0]);
        var term1 = Visit(terms[1]);

        return ctx => new Let(info, "_", term0(ctx), term1(ctx.AddName("_")));
    }

    public override Func<Context, ITerm> VisitTerm_unpack(FullPolyParser.Term_unpackContext context)
    {
        var info = context.GetFileInfo();
        var ucid = context.UCID().GetText();
        var lcid = context.LCID().GetText();
        var terms = context.term();
        var t1 = Visit(terms[0]);
        var t2 = Visit(terms[1]);

        return ctx =>
        {
            var ctx1 = ctx.AddName(ucid);
            var ctx2 = ctx1.AddName(lcid);
            return new Unpack(info, ucid, lcid, t1(ctx), t2(ctx2));
        };
    }

    public override Func<Context, ITerm> VisitTerm_letrec(FullPolyParser.Term_letrecContext context)
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

    public override Func<Context, ITerm> VisitAppterm_times(FullPolyParser.Appterm_timesContext context)
    {
        var info = context.GetFileInfo();
        var pathTerms = context.pathterm();
        var p0 = Visit(pathTerms[0]);
        var p1 = Visit(pathTerms[1]);

        return ctx => new TimesFloat(info, p0(ctx), p1(ctx));
    }

    public override Func<Context, ITerm> VisitAppterm_iszero(FullPolyParser.Appterm_iszeroContext context)
    {
        var info = context.GetFileInfo();
        var pathTerm = Visit(context.pathterm());

        return ctx => new IsZero(info, pathTerm(ctx));
    }

    public override Func<Context, ITerm> VisitAppterm_path(FullPolyParser.Appterm_pathContext context) => Visit(context.pathterm());

    public override Func<Context, ITerm> VisitAppterm_app_path(FullPolyParser.Appterm_app_pathContext context)
    {
        var info = context.GetFileInfo();
        var a = Visit(context.appterm());
        var p = Visit(context.pathterm());

        return ctx => new App(info, a(ctx), p(ctx));
    }

    public override Func<Context, ITerm> VisitAppterm_tapp(FullPolyParser.Appterm_tappContext context)
    {
        var info = context.GetFileInfo();
        var app = Visit(context.appterm());
        var type = _typeVisitor.Visit(context.type());
        return ctx => new TApp(info, app(ctx), type(ctx));
    }

    public override Func<Context, ITerm> VisitAppterm_succ(FullPolyParser.Appterm_succContext context)
    {
        var info = context.GetFileInfo();
        var pathTerm = Visit(context.pathterm());

        return ctx => new Succ(info, pathTerm(ctx));
    }

    public override Func<Context, ITerm> VisitAppterm_pred(FullPolyParser.Appterm_predContext context)
    {
        var info = context.GetFileInfo();
        var pathTerm = Visit(context.pathterm());

        return ctx => new Pred(info, pathTerm(ctx));
    }

    public override Func<Context, ITerm> VisitAppterm_fix(FullPolyParser.Appterm_fixContext context)
    {
        var info = context.GetFileInfo();
        var pathTerm = Visit(context.pathterm());

        return ctx => new Fix(info, pathTerm(ctx));
    }

    public override Func<Context, ITerm> VisitAscribeterm_aaa(FullPolyParser.Ascribeterm_aaaContext context)
    {
        var info = context.GetFileInfo();
        var at = Visit(context.aterm());
        var type = _typeVisitor.Visit(context.type());
        return ctx => new Ascribe(info, at(ctx), type(ctx));
    }

    public override Func<Context, ITerm> VisitAscribeterm_a(FullPolyParser.Ascribeterm_aContext context) => Visit(context.aterm());

    public override Func<Context, ITerm> VisitPathterm_intv(FullPolyParser.Pathterm_intvContext context)
    {
        var info = context.GetFileInfo();
        var pt = Visit(context.pathterm());
        var intv = context.INTV().GetText();
        return c => new Proj(info, pt(c), intv);
    }

    public override Func<Context, ITerm> VisitPathterm_lcid(FullPolyParser.Pathterm_lcidContext context)
    {
        var info = context.GetFileInfo();
        var pt = Visit(context.pathterm());
        var lcid = context.LCID().GetText();
        return c => new Proj(info, pt(c), lcid);
    }

    public override Func<Context, ITerm> VisitPathterm_asterm(FullPolyParser.Pathterm_astermContext context) => Visit(context.ascribeterm());

    public override Func<Context, ITerm> VisitTermseq_term(FullPolyParser.Termseq_termContext context) => Visit(context.term());

    public override Func<Context, ITerm> VisitTermseq_termseq(FullPolyParser.Termseq_termseqContext context)
    {
        var info = context.GetFileInfo();
        var t = Visit(context.term());
        var ts = Visit(context.termseq());

        return c => new App(info, new Abs(info, "_", new TypeUnit(), ts(c.AddName("_"))), t(c));
    }

    public override Func<Context, ITerm> VisitAterm_paren(FullPolyParser.Aterm_parenContext context) => Visit(context.termseq());

    public override Func<Context, ITerm> VisitAterm_inert(FullPolyParser.Aterm_inertContext context)
    {
        var info = context.GetFileInfo();
        var type = _typeVisitor.Visit(context.type());
        return c => new Inert(info, type(c));
    }

    public override Func<Context, ITerm> VisitAterm_true(FullPolyParser.Aterm_trueContext context)
    {
        var info = context.GetFileInfo();
        return _ => new True(info);
    }

    public override Func<Context, ITerm> VisitAterm_false(FullPolyParser.Aterm_falseContext context)
    {
        var info = context.GetFileInfo();
        return _ => new False(info);
    }

    public override Func<Context, ITerm> VisitAterm_lt(FullPolyParser.Aterm_ltContext context)
    {
        var info = context.GetFileInfo();
        var v = context.LCID().GetText();
        var term = Visit(context.term());
        var type = _typeVisitor.Visit(context.type());
        return c => new Tag(info, v, term(c), type(c));
    }

    public override Func<Context, ITerm> VisitAterm_pack(FullPolyParser.Aterm_packContext context)
    {
        var info = context.GetFileInfo();
        var types = context.type();
        var ty1 = _typeVisitor.Visit(types[0]);
        var term = Visit(context.term());
        var ty2 = _typeVisitor.Visit(types[1]);
        return ctx => new Pack(info, ty1(ctx), term(ctx), ty2(ctx));
    }

    public override Func<Context, ITerm> VisitAterm_lcid(FullPolyParser.Aterm_lcidContext context)
    {
        var info = context.GetFileInfo();
        var v = context.LCID().GetText();

        return c => new Var(info, c.NameToIndex(v), c.Length);
    }

    public override Func<Context, ITerm> VisitAterm_stringv(FullPolyParser.Aterm_stringvContext context)
    {
        var info = context.GetFileInfo();
        var v = context.STRINGV().GetText().Trim('"');
        return _ => new StringTerm(v);
    }

    public override Func<Context, ITerm> VisitAterm_unit(FullPolyParser.Aterm_unitContext context)
    {
        var info = context.GetFileInfo();
        return _ => new Unit(info);
    }

    public override Func<Context, ITerm> VisitAterm_fields(FullPolyParser.Aterm_fieldsContext context)
    {
        var info = context.GetFileInfo();
        var fields = _fieldsVisitor.Visit(context.fields());

        return c => new Record(info, fields((c, 1)).ToList());
    }

    public override Func<Context, ITerm> VisitAterm_floatv(FullPolyParser.Aterm_floatvContext context)
    {
        var info = context.GetFileInfo();
        var value = double.Parse(context.FLOATV().GetText(), System.Globalization.CultureInfo.InvariantCulture);
        return _ => new Float(info, value);
    }

    public override Func<Context, ITerm> VisitAterm_intv(FullPolyParser.Aterm_intvContext context)
    {
        var info = context.GetFileInfo();
        var value = int.Parse(context.INTV().GetText(), System.Globalization.CultureInfo.InvariantCulture);
        return _ => BuildNatTerm(value);

        ITerm BuildNatTerm(int n) => n == 0 ? new Zero(info) : new Succ(info, BuildNatTerm(n - 1));
    }
}
