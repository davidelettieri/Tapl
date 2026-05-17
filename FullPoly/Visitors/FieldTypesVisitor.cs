using System;
using System.Collections.Generic;
using System.Linq;
using Common;

namespace FullPoly.Visitors;

public sealed class FieldTypesVisitor(TypeVisitor typeVisitor)
    : FullPolyBaseVisitor<Func<Context, int, IEnumerable<(string, IType)>>>
{
    private readonly NEFieldTypesVisitor _nefieldTypesVisitor = new(typeVisitor);

    public override Func<Context, int, IEnumerable<(string, IType)>> VisitFieldtypes_nefieldtypes(
        FullPolyParser.Fieldtypes_nefieldtypesContext context)
    {
        if (context.nefieldtypes() is null)
            return (ctx, i) => Enumerable.Empty<(string, IType)>();

        var nefieldTypes = _nefieldTypesVisitor.Visit(context.nefieldtypes());

        return (ctx, i) => nefieldTypes(ctx, i);
    }
}