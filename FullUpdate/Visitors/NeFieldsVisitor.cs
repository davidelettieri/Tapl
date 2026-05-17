using System;
using System.Collections.Generic;
using Common;
using FullUpdate.Syntax;
using FullUpdate.Syntax.Terms;

namespace FullUpdate.Visitors;

public sealed class NeFieldsVisitor : FullUpdateBaseVisitor<Func<Context, int, List<(string, Variance, ITerm)>>>
{
    private readonly FieldVisitor _fieldVisitor;

    public NeFieldsVisitor(TermVisitor termVisitor)
    {
        _fieldVisitor = new FieldVisitor(termVisitor);
    }

    public override Func<Context, int, List<(string, Variance, ITerm)>> VisitNefields_cons(
        FullUpdateParser.Nefields_consContext context)
    {
        var head = _fieldVisitor.Visit(context.field());
        var tail = Visit(context.nefields());
        return (ctx, i) =>
        {
            var h = head(ctx, i);
            var t = tail(ctx, i + 1);
            t.Insert(0, h);
            return t;
        };
    }

    public override Func<Context, int, List<(string, Variance, ITerm)>> VisitNefields_one(
        FullUpdateParser.Nefields_oneContext context)
    {
        var field = _fieldVisitor.Visit(context.field());
        return (ctx, i) => new List<(string, Variance, ITerm)> { field(ctx, i) };
    }
}
