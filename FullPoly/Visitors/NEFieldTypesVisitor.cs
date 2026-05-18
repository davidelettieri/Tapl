using System;
using System.Collections.Generic;
using System.Linq;
using Common;

namespace FullPoly.Visitors;

public sealed class NEFieldTypesVisitor(TypeVisitor typeVisitor)
    : FullPolyBaseVisitor<Func<Context, int, IEnumerable<(string, IType)>>>
{
    private readonly FieldTypeVisitor _fieldTypesVisitor = new(typeVisitor);

    public override Func<Context, int, IEnumerable<(string, IType)>> VisitNefieldtypes_fieldtype(
        FullPolyParser.Nefieldtypes_fieldtypeContext context)
    {
        var fieldType = _fieldTypesVisitor.Visit(context.fieldtype());

        return (ctx, i) => Enumerable.Repeat(fieldType(ctx, i), 1);
    }

    public override Func<Context, int, IEnumerable<(string, IType)>> VisitNefieldtypes_nefieldtype(
        FullPolyParser.Nefieldtypes_nefieldtypeContext context)
    {
        var fieldType = _fieldTypesVisitor.Visit(context.fieldtype());
        var nefieldTypes = Visit(context.nefieldtypes());

        return (ctx, i) => Enumerable.Repeat(fieldType(ctx, i), 1).Concat(nefieldTypes(ctx, i + 1));
    }
}