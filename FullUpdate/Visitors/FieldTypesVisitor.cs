using System;
using System.Collections.Generic;
using System.Linq;
using Common;
using FullUpdate.Syntax;

namespace FullUpdate.Visitors;

public sealed class FieldTypesVisitor(TypeVisitor typeVisitor)
    : FullUpdateBaseVisitor<Func<Context, int, IEnumerable<(string, Variance, IType)>>>
{
    private readonly NeFieldTypesVisitor _neFieldTypesVisitor = new(typeVisitor);

    public override Func<Context, int, IEnumerable<(string, Variance, IType)>> VisitFieldtypes_ne(
        FullUpdateParser.Fieldtypes_neContext context)
        => _neFieldTypesVisitor.Visit(context.nefieldtypes());

    public override Func<Context, int, IEnumerable<(string, Variance, IType)>> VisitFieldtypes_empty(
        FullUpdateParser.Fieldtypes_emptyContext context)
        => (_, _) => Enumerable.Empty<(string, Variance, IType)>();
}
