using Antlr4.Runtime.Misc;
using Common;
using System;

namespace FullSimple.Visitors;

public sealed class FieldVisitor(TermVisitor termVisitor) : FullSimpleBaseVisitor<Func<(Context, int), (string, ITerm)>>
{
    public override Func<(Context, int), (string, ITerm)> VisitField_lcid(FullSimpleParser.Field_lcidContext context)
    {
        var id = context.LCID().GetText();
        var term = termVisitor.Visit(context.term());

        return t => (id, term(t.Item1));
    }

    public override Func<(Context, int), (string, ITerm)> VisitField_term(FullSimpleParser.Field_termContext context)
    {
        var term = termVisitor.Visit(context);

        return arg => (arg.Item2.ToString(), term(arg.Item1));
    }
}