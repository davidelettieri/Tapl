using System;
using System.Collections.Generic;
using System.Linq;
using Common;

namespace FullSimple.Visitors;

public sealed class CasesVisitor(TermVisitor termVisitor)
    : FullSimpleBaseVisitor<Func<Context, IEnumerable<(string, string, ITerm)>>>
{
    private readonly CaseVisitor _caseVisitor = new(termVisitor);

    public override Func<Context, IEnumerable<(string, string, ITerm)>> VisitCases_case(
        FullSimpleParser.Cases_caseContext context)
    {
        var c = _caseVisitor.Visit(context.@case());

        return ctx => Enumerable.Repeat(c(ctx), 1);
    }

    public override Func<Context, IEnumerable<(string, string, ITerm)>> VisitCases_case_vbar_cases(
        FullSimpleParser.Cases_case_vbar_casesContext context)
    {
        var c = _caseVisitor.Visit(context.@case());
        var cases = Visit(context.cases());

        return ctx => Enumerable.Repeat(c(ctx), 1).Concat(cases(ctx));
    }
}