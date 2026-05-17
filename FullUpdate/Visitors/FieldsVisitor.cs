using System;
using System.Collections.Generic;
using System.Linq;
using Common;
using FullUpdate.Syntax;
using FullUpdate.Syntax.Terms;

namespace FullUpdate.Visitors;

public sealed class FieldsVisitor : FullUpdateBaseVisitor<Func<Context, int, List<(string, Variance, ITerm)>>>
{
    private readonly NeFieldsVisitor _neFieldsVisitor;

    public FieldsVisitor(TermVisitor termVisitor)
    {
        _neFieldsVisitor = new NeFieldsVisitor(termVisitor);
    }

    public override Func<Context, int, List<(string, Variance, ITerm)>> VisitFields_ne(
        FullUpdateParser.Fields_neContext context)
        => _neFieldsVisitor.Visit(context.nefields());

    public override Func<Context, int, List<(string, Variance, ITerm)>> VisitFields_empty(
        FullUpdateParser.Fields_emptyContext context)
        => (_, _) => new List<(string, Variance, ITerm)>();
}
