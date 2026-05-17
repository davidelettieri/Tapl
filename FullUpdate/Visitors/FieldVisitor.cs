using System;
using Common;
using FullUpdate.Syntax;
using FullUpdate.Syntax.Terms;

namespace FullUpdate.Visitors;

public sealed class FieldVisitor : FullUpdateBaseVisitor<Func<Context, int, (string, Variance, ITerm)>>
{
    private readonly TermVisitor _termVisitor;

    public FieldVisitor(TermVisitor termVisitor) => _termVisitor = termVisitor;

    public override Func<Context, int, (string, Variance, ITerm)> VisitField_hash_lcid(
        FullUpdateParser.Field_hash_lcidContext context)
    {
        var name = context.LCID().GetText();
        var term = _termVisitor.Visit(context.term());
        return (ctx, _) => (name, Variance.Invariant, term(ctx));
    }

    public override Func<Context, int, (string, Variance, ITerm)> VisitField_hash_term(
        FullUpdateParser.Field_hash_termContext context)
    {
        var term = _termVisitor.Visit(context.term());
        return (ctx, i) => (i.ToString(), Variance.Invariant, term(ctx));
    }

    public override Func<Context, int, (string, Variance, ITerm)> VisitField_lcid(
        FullUpdateParser.Field_lcidContext context)
    {
        var name = context.LCID().GetText();
        var term = _termVisitor.Visit(context.term());
        return (ctx, _) => (name, Variance.Covariant, term(ctx));
    }

    public override Func<Context, int, (string, Variance, ITerm)> VisitField_term(
        FullUpdateParser.Field_termContext context)
    {
        var term = _termVisitor.Visit(context.term());
        return (ctx, i) => (i.ToString(), Variance.Covariant, term(ctx));
    }
}
