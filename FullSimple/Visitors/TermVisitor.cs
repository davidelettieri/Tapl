using Antlr4.Runtime.Misc;
using Common;
using System;
using FullSimple.Syntax.Terms;
using static FullSimple.Helper;
using System.IO;

namespace FullSimple.Visitors
{
    public class TermVisitor : FullSimpleBaseVisitor<Func<Context, ITerm>>
    {
        private static readonly AppTermVisitor _appTermVisitor = new AppTermVisitor();
        private static readonly CasesVisitor _casesVisitor = new CasesVisitor();
        private static readonly TypeVisitor _typeVisitor = new TypeVisitor();

        public override Func<Context, ITerm> VisitTerm_appterm([NotNull] FullSimpleParser.Term_apptermContext context)
        {
            return _appTermVisitor.Visit(context);
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
            return base.VisitAppterm_succ(context);
        }

        public override Func<Context, ITerm> VisitAppterm_pred([NotNull] FullSimpleParser.Appterm_predContext context)
        {
            return base.VisitAppterm_pred(context);
        }

        public override Func<Context, ITerm> VisitAppterm_fix([NotNull] FullSimpleParser.Appterm_fixContext context)
        {
            return base.VisitAppterm_fix(context);
        }
    }
}
