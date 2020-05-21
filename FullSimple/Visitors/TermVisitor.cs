using Antlr4.Runtime.Misc;
using Common;
using System;
using FullSimple.Syntax.Terms;
using static FullSimple.Helper;
using FullSimple.Syntax.Types;
using System.Collections.Generic;
using System.Linq;

namespace FullSimple.Visitors
{
    public class TermVisitor : FullSimpleBaseVisitor<Func<Context, ITerm>>
    {
        private static readonly CasesVisitor _casesVisitor = new CasesVisitor();
        private static readonly TypeVisitor _typeVisitor = new TypeVisitor();
        private readonly FieldsVisitor _fieldsVisitor;

        public TermVisitor()
        {
            _fieldsVisitor = new FieldsVisitor(this);
        }

        public override Func<Context, ITerm> VisitTerm_appterm([NotNull] FullSimpleParser.Term_apptermContext context)
        {
            return Visit(context.appterm());
        }

        public override Func<Context, ITerm> VisitTerm_ift([NotNull] FullSimpleParser.Term_iftContext context)
        {
            var terms = context.term();
            var condition = Visit(terms[0]);
            var then = Visit(terms[1]);
            var @else = Visit(terms[2]);
            var info = GetFileInfo(context);
            return c => new If(info, condition(c), then(c), @else(c));
        }

        public override Func<Context, ITerm> VisitTerm_caseOf([NotNull] FullSimpleParser.Term_caseOfContext context)
        {
            var info = GetFileInfo(context);
            var term = Visit(context.term());
            var @case = _casesVisitor.Visit(context.cases());
            return ctx => new Case(info, term(ctx), @case(ctx));
        }

        public override Func<Context, ITerm> VisitTerm_llcid([NotNull] FullSimpleParser.Term_llcidContext context)
        {
            var v = context.LCID().GetText();
            var info = GetFileInfo(context);
            var type = _typeVisitor.Visit(context.type());
            var term = Visit(context.term());
            return ctx =>
            {
                var ctx1 = ctx.AddName(v);
                return new Abs(info, v, type(ctx), term(ctx1));
            };
        }

        public override Func<Context, ITerm> VisitTerm_luc([NotNull] FullSimpleParser.Term_lucContext context)
        {
            var info = GetFileInfo(context);
            var type = _typeVisitor.Visit(context.type());
            var term = Visit(context.term());
            return ctx =>
            {
                var ctx1 = ctx.AddName("_");
                return new Abs(info, "_", type(ctx), term(ctx1));
            };
        }

        public override Func<Context, ITerm> VisitTerm_ll([NotNull] FullSimpleParser.Term_llContext context)
        {
            var info = GetFileInfo(context);
            var v = context.LCID().GetText();
            var terms = context.term();
            var term0 = Visit(terms[0]);
            var term1 = Visit(terms[1]);

            return ctx => new Let(info, v, term0(ctx), term1(ctx.AddName(v)));
        }

        public override Func<Context, ITerm> VisitTerm_lu([NotNull] FullSimpleParser.Term_luContext context)
        {
            var info = GetFileInfo(context);
            var terms = context.term();
            var term0 = Visit(terms[0]);
            var term1 = Visit(terms[1]);

            return ctx => new Let(info, "_", term0(ctx), term1(ctx.AddName("_")));
        }

        public override Func<Context, ITerm> VisitTerm_letrec([NotNull] FullSimpleParser.Term_letrecContext context)
        {
            var info = GetFileInfo(context);
            var v = context.LCID().GetText();
            var type = _typeVisitor.Visit(context.type());
            var terms = context.term();
            var eqTerm = Visit(terms[0]);
            var inTerm = Visit(terms[1]);

            return ctx =>
            {
                var ctx1 = ctx.AddName(v);
                var abs = new Abs(info, v, type(ctx), eqTerm(ctx1));
                var fix = new Fix(info, abs);
                return new Let(info, v, fix, inTerm(ctx));
            };
        }

        public override Func<Context, ITerm> VisitAppterm_times([NotNull] FullSimpleParser.Appterm_timesContext context)
        {
            var info = GetFileInfo(context);
            var pathTerms = context.pathterm();
            var p0 = Visit(pathTerms[0]);
            var p1 = Visit(pathTerms[1]);

            return ctx => new TimesFloat(info, p0(ctx), p1(ctx));
        }

        public override Func<Context, ITerm> VisitAppterm_iszero([NotNull] FullSimpleParser.Appterm_iszeroContext context)
        {
            var info = GetFileInfo(context);
            var pathTerm = Visit(context.pathterm());

            return ctx => new IsZero(info, pathTerm(ctx));
        }

        public override Func<Context, ITerm> VisitAppterm_path([NotNull] FullSimpleParser.Appterm_pathContext context)
        {
            return Visit(context.pathterm());
        }

        public override Func<Context, ITerm> VisitAppterm_app_path([NotNull] FullSimpleParser.Appterm_app_pathContext context)
        {
            var info = GetFileInfo(context);
            var a = Visit(context.appterm());
            var p = Visit(context.pathterm());

            return ctx => new App(info, a(ctx), p(ctx));
        }

        public override Func<Context, ITerm> VisitAppterm_succ([NotNull] FullSimpleParser.Appterm_succContext context)
        {
            var info = GetFileInfo(context);
            var pathTerm = Visit(context.pathterm());

            return ctx => new Succ(info, pathTerm(ctx));
        }

        public override Func<Context, ITerm> VisitAppterm_pred([NotNull] FullSimpleParser.Appterm_predContext context)
        {
            var info = GetFileInfo(context);
            var pathTerm = Visit(context.pathterm());

            return ctx => new Pred(info, pathTerm(ctx));
        }

        public override Func<Context, ITerm> VisitAppterm_fix([NotNull] FullSimpleParser.Appterm_fixContext context)
        {
            var info = GetFileInfo(context);
            var pathTerm = Visit(context.pathterm());

            return ctx => new Fix(info, pathTerm(ctx));
        }

        public override Func<Context, ITerm> VisitAscribeterm_aaa([NotNull] FullSimpleParser.Ascribeterm_aaaContext context)
        {
            var info = GetFileInfo(context);
            var at = Visit(context.aterm());
            var type = _typeVisitor.Visit(context.type());
            return ctx => new Ascribe(info, at(ctx), type(ctx));
        }

        public override Func<Context, ITerm> VisitAscribeterm_a([NotNull] FullSimpleParser.Ascribeterm_aContext context)
        {
            return Visit(context.aterm());
        }

        public override Func<Context, ITerm> VisitPathterm_intv([NotNull] FullSimpleParser.Pathterm_intvContext context)
        {
            var info = GetFileInfo(context);
            var pt = Visit(context.pathterm());
            var intv = context.INTV().GetText();
            return c => new Proj(info, pt(c), intv);
        }

        public override Func<Context, ITerm> VisitPathterm_lcid([NotNull] FullSimpleParser.Pathterm_lcidContext context)
        {
            var info = GetFileInfo(context);
            var pt = Visit(context.pathterm());
            var lcid = context.LCID().GetText();
            return c => new Proj(info, pt(c), lcid);
        }

        public override Func<Context, ITerm> VisitPathterm_asterm([NotNull] FullSimpleParser.Pathterm_astermContext context)
        {
            return Visit(context.ascribeterm());
        }

        public override Func<Context, ITerm> VisitTermseq_term([NotNull] FullSimpleParser.Termseq_termContext context)
        {
            return Visit(context.term());
        }

        public override Func<Context, ITerm> VisitTermseq_termseq([NotNull] FullSimpleParser.Termseq_termseqContext context)
        {
            var info = GetFileInfo(context);
            var t = Visit(context.term());
            var ts = Visit(context.termseq());

            return c => new App(info, new Abs(info, "_", TypeUnit.Instance, ts(c.AddName("_"))), t(c));
        }

        public override Func<Context, ITerm> VisitAterm_paren([NotNull] FullSimpleParser.Aterm_parenContext context)
        {
            return Visit(context.termseq());
        }

        public override Func<Context, ITerm> VisitAterm_inert([NotNull] FullSimpleParser.Aterm_inertContext context)
        {
            var info = GetFileInfo(context);
            var type = _typeVisitor.Visit(context.type());
            return c => new Inert(info, type(c));
        }

        public override Func<Context, ITerm> VisitAterm_true([NotNull] FullSimpleParser.Aterm_trueContext context)
        {
            var info = GetFileInfo(context);
            return _ => new True(info);
        }

        public override Func<Context, ITerm> VisitAterm_false([NotNull] FullSimpleParser.Aterm_falseContext context)
        {
            var info = GetFileInfo(context);
            return _ => new False(info);
        }

        public override Func<Context, ITerm> VisitAterm_lt([NotNull] FullSimpleParser.Aterm_ltContext context)
        {
            var info = GetFileInfo(context);
            var v = context.LCID().GetText();
            var term = Visit(context.term());
            var type = _typeVisitor.Visit(context.type());
            return c => new Tag(info, v, term(c), type(c));
        }

        public override Func<Context, ITerm> VisitAterm_lcid([NotNull] FullSimpleParser.Aterm_lcidContext context)
        {
            var info = GetFileInfo(context);
            var v = context.LCID().GetText();

            return c => new Var(info, c.NameToIndex(v), c.Length);
        }

        public override Func<Context, ITerm> VisitAterm_stringv([NotNull] FullSimpleParser.Aterm_stringvContext context)
        {
            var info = GetFileInfo(context);
            var v = context.STRINGV().GetText();
            return _ => new StringTerm(v);
        }

        public override Func<Context, ITerm> VisitAterm_unit([NotNull] FullSimpleParser.Aterm_unitContext context)
        {
            var info = GetFileInfo(context);
            return _ => new Unit(info);
        }

        public override Func<Context, ITerm> VisitAterm_fields([NotNull] FullSimpleParser.Aterm_fieldsContext context)
        {
            var info = GetFileInfo(context);
            var fields = _fieldsVisitor.Visit(context.fields());

            return c => new Record(info, fields((c, 1)));
        }

        public override Func<Context, ITerm> VisitAterm_floatv([NotNull] FullSimpleParser.Aterm_floatvContext context)
        {
            var info = GetFileInfo(context);
            var value = float.Parse(context.FLOATV().GetText(), System.Globalization.CultureInfo.InvariantCulture);
            return _ => new Float(info, value);
        }

        public override Func<Context, ITerm> VisitAterm_intv([NotNull] FullSimpleParser.Aterm_intvContext context)
        {
            var info = GetFileInfo(context);
            var value = int.Parse(context.INTV().GetText(), System.Globalization.CultureInfo.InvariantCulture);
            return _ => f(value);

            ITerm f(int n) => n == 0 ? new Zero(info) as ITerm : new Succ(info, f(n - 1));
        }
    }

    public class FieldsVisitor : FullSimpleBaseVisitor<Func<(Context, int), IEnumerable<(string, ITerm)>>>
    {
        private readonly FieldVisitor _fieldVisitor;

        public FieldsVisitor(TermVisitor termVisitor)
        {
            _fieldVisitor = new FieldVisitor(termVisitor);
        }

        public override Func<(Context, int), IEnumerable<(string, ITerm)>> VisitFields([NotNull] FullSimpleParser.FieldsContext context)
        {
            var neFields = context.nefields();

            if (neFields is null)
                return _ => Enumerable.Empty<(string, ITerm)>();

            return Visit(neFields);
        }

        public override Func<(Context, int), IEnumerable<(string, ITerm)>> VisitNefields_field([NotNull] FullSimpleParser.Nefields_fieldContext context)
        {
            var info = GetFileInfo(context);
            var field = _fieldVisitor.Visit(context.field());

            return c => Wrap(field(c));
        }

        public override Func<(Context, int), IEnumerable<(string, ITerm)>> VisitNefields_field_comma_nefields([NotNull] FullSimpleParser.Nefields_field_comma_nefieldsContext context)
        {
            var info = GetFileInfo(context);
            var field = _fieldVisitor.Visit(context.field());
            var nefields = Visit(context.nefields());
            return t => Wrap(field(t)).Concat(nefields(t));
        }

        private static IEnumerable<(string, ITerm)> Wrap((string, ITerm) s)
            => Enumerable.Repeat(s, 1);
    }

    public class FieldVisitor : FullSimpleBaseVisitor<Func<(Context, int), (string, ITerm)>>
    {
        private readonly TermVisitor _termVisitor;

        public FieldVisitor(TermVisitor termVisitor)
        {
            _termVisitor = termVisitor;
        }

        public override Func<(Context, int), (string, ITerm)> VisitField_lcid([NotNull] FullSimpleParser.Field_lcidContext context)
        {
            var id = context.LCID().GetText();
            var term = _termVisitor.Visit(context.term());

            return t => (id, term(t.Item1));
        }

        public override Func<(Context, int), (string, ITerm)> VisitField_term([NotNull] FullSimpleParser.Field_termContext context)
        {
            return base.VisitField_term(context);
        }
    }
}
