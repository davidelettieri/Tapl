using Antlr4.Runtime.Misc;
using Common;
using System;

namespace FullSimple.Visitors
{
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
