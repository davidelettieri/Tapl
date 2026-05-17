using System;
using System.Collections.Generic;
using System.Linq;
using Common;
using FullUpdate.Syntax;

namespace FullUpdate.Visitors;

public sealed class NeFieldTypesVisitor(TypeVisitor typeVisitor)
    : FullUpdateBaseVisitor<Func<Context, int, IEnumerable<(string, Variance, IType)>>>
{
    private readonly FieldTypeVisitor _fieldTypeVisitor = new(typeVisitor);

    public override Func<Context, int, IEnumerable<(string, Variance, IType)>> VisitNefieldtypes_cons(
        FullUpdateParser.Nefieldtypes_consContext context)
    {
        var head = _fieldTypeVisitor.Visit(context.fieldtype());
        var tail = Visit(context.nefieldtypes());
        return (ctx, i) => new[] { head(ctx, i) }.Concat(tail(ctx, i + 1));
    }

    public override Func<Context, int, IEnumerable<(string, Variance, IType)>> VisitNefieldtypes_one(
        FullUpdateParser.Nefieldtypes_oneContext context)
    {
        var field = _fieldTypeVisitor.Visit(context.fieldtype());
        return (ctx, i) => new[] { field(ctx, i) };
    }
}
