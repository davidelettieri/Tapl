using Antlr4.Runtime.Atn;
using Antlr4.Runtime.Misc;
using Common;
using FullSimple.Syntax.Terms;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FullSimple.Visitors
{
    public class CasesVisitor : FullSimpleBaseVisitor<Func<Context, IEnumerable<(string, string, ITerm)>>>
    {
        private readonly CaseVisitor _caseVisitor = new CaseVisitor();
        public override Func<Context, IEnumerable<(string, string, ITerm)>> VisitCases_case([NotNull] FullSimpleParser.Cases_caseContext context)
        {
            var c = _caseVisitor.Visit(context.@case());

            return ctx => Enumerable.Repeat(c(ctx), 1);
        }

        public override Func<Context, IEnumerable<(string, string, ITerm)>> VisitCases_case_vbar_cases([NotNull] FullSimpleParser.Cases_case_vbar_casesContext context)
        {
            var c = _caseVisitor.Visit(context.@case());
            var @cases = Visit(context.cases());

            return ctx => Enumerable.Repeat(c(ctx), 1).Concat(@cases(ctx));
        }
    }
}
