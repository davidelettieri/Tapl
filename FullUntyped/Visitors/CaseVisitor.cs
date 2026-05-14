using System;
using Common;

namespace FullUntyped.Visitors;

public sealed class CaseVisitor(TermVisitor termVisitor) : FullUntypedBaseVisitor<Func<Context, (string, string, ITerm)>>
{
    public override Func<Context, (string, string, ITerm)> VisitCase(FullUntypedParser.CaseContext context)
    {
        var lcid0 = context.LCID(0).GetText();
        var lcid1 = context.LCID(1).GetText();
        var term = termVisitor.Visit(context.appterm());
        return ctx =>
        {
            var ctx1 = ctx.AddName(lcid1);
            return (lcid0, lcid1, term(ctx1));
        };
    }
}