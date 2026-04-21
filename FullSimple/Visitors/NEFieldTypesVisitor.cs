using System;
using System.Collections.Generic;
using System.Linq;
using Common;

namespace FullSimple.Visitors;

public sealed class NEFieldTypesVisitor(TypeVisitor typeVisitor)
    : FullSimpleBaseVisitor<Func<Context, int, IEnumerable<(string, IType)>>>
{
    private readonly FieldTypeVisitor _fieldTypesVisitor = new(typeVisitor);

    public override Func<Context, int, IEnumerable<(string, IType)>> VisitNefieldtypes_fieldtype(
        FullSimpleParser.Nefieldtypes_fieldtypeContext context)
    {
        var fieldType = _fieldTypesVisitor.Visit(context.fieldtype());

        return (ctx, i) => Enumerable.Repeat(fieldType(ctx, i), 1);
    }

    public override Func<Context, int, IEnumerable<(string, IType)>> VisitNefieldtypes_nefieldtype(
        FullSimpleParser.Nefieldtypes_nefieldtypeContext context)
    {
        var fieldType = _fieldTypesVisitor.Visit(context.fieldtype());
        var nefieldTypes = Visit(context.nefieldtypes());

        return (ctx, i) => Enumerable.Repeat(fieldType(ctx, i), 1).Concat(nefieldTypes(ctx, i + 1));
    }
}