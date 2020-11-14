using Antlr4.Runtime.Misc;
using Common;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FullSimple.Visitors
{
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
            var info = context.GetFileInfo();
            var field = _fieldVisitor.Visit(context.field());

            return c => Wrap(field(c));
        }

        public override Func<(Context, int), IEnumerable<(string, ITerm)>> VisitNefields_field_comma_nefields([NotNull] FullSimpleParser.Nefields_field_comma_nefieldsContext context)
        {
            var info = context.GetFileInfo();
            var field = _fieldVisitor.Visit(context.field());
            var nefields = Visit(context.nefields());
            return t =>
            {
                var ft = field(t);
                var fields = nefields((t.Item1, t.Item2 + 1));
                return Wrap(ft).Concat(fields);
            };
        }

        private static IEnumerable<(string, ITerm)> Wrap((string, ITerm) s)
            => Enumerable.Repeat(s, 1);
    }
}
