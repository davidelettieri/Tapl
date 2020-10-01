using Antlr4.Runtime.Misc;
using Common;
using System;

namespace FullSimple.Visitors
{
    public class CaseVisitor : FullSimpleBaseVisitor<Func<Context, (string, string, ITerm)>>
    {
        private readonly TermVisitor _termVisitor = new TermVisitor();

        public override Func<Context, (string, string, ITerm)> VisitCase([NotNull] FullSimpleParser.CaseContext context)
        {
            var lcid0 = context.LCID(0).GetText();
            var lcid1 = context.LCID(1).GetText();
            var term = _termVisitor.Visit(context.appterm());
            return ctx =>
            {
                var ctx1 = ctx.AddName(lcid1);
                return (lcid0, lcid1, term(ctx1));
            };
        }
    }
}
