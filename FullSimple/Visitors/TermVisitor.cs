using Antlr4.Runtime;
using Antlr4.Runtime.Misc;
using FullSimple.Syntax;
using Common;
using System;
using System.Collections.Immutable;
using FullSimple.Syntax.Terms;
using FullSimple.Syntax.Types;
using static FullSimple.Helper;
using FullSimple.Command;
using System.Collections;
using System.Collections.Generic;

namespace FullSimple.Visitors
{
    public class AppTermVisitor : FullSimpleBaseVisitor<Func<Context, ITerm>>
    {

    }

    public class CasesVisitor : FullSimpleBaseVisitor<Func<Context, IEnumerable<(string, string, ITerm)>>>
    {

    }

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

        }

        public override Func<Context, ITerm> VisitTerm_letrec([NotNull] FullSimpleParser.Term_letrecContext context)
        {
            return base.VisitTerm_letrec(context);
        }



        public override Func<Context, ITerm> VisitAbs([NotNull] FullSimpleParser.AbsContext context)
        {
            var boundVar = context.VAR().GetText();
            var body = Visit(context.term());
            var info = GetFileInfo(context);
            ITerm result(Context c) => new Abs(info, body(c.AddName(boundVar)), boundVar, _typeVisitor.Visit(context.type()));
            return result;
        }
    }
}
