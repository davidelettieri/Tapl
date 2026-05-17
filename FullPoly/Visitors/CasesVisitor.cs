using System;
using System.Collections.Generic;
using System.Linq;
using Common;

namespace FullPoly.Visitors;

public sealed class CasesVisitor(TermVisitor termVisitor)
    : FullPolyBaseVisitor<Func<Context, IEnumerable<(string, string, ITerm)>>>
{
    private readonly CaseVisitor _caseVisitor = new(termVisitor);

    public override Func<Context, IEnumerable<(string, string, ITerm)>> VisitCases_case(
        FullPolyParser.Cases_caseContext context)
    {
        var c = _caseVisitor.Visit(context.@case());

        return ctx => Enumerable.Repeat(c(ctx), 1);
    }

    public override Func<Context, IEnumerable<(string, string, ITerm)>> VisitCases_case_vbar_cases(
        FullPolyParser.Cases_case_vbar_casesContext context)
    {
        var c = _caseVisitor.Visit(context.@case());
        var cases = Visit(context.cases());

        return ctx => Enumerable.Repeat(c(ctx), 1).Concat(cases(ctx));
    }
}