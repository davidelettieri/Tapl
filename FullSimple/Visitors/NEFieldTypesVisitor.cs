using Antlr4.Runtime.Misc;
using Common;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FullSimple.Visitors;

public class NEFieldTypesVisitor : FullSimpleBaseVisitor<Func<Context, int, IEnumerable<(string, IType)>>>
{
    private readonly FieldTypeVisitor _fieldTypesVisitor;

    public NEFieldTypesVisitor(TypeVisitor typeVisitor)
    {
            _fieldTypesVisitor = new FieldTypeVisitor(typeVisitor);
        }

    public override Func<Context, int, IEnumerable<(string, IType)>> VisitNefieldtypes_fieldtype([NotNull] FullSimpleParser.Nefieldtypes_fieldtypeContext context)
    {
            var fieldType = _fieldTypesVisitor.Visit(context.fieldtype());

            return (ctx, i) => Enumerable.Repeat(fieldType(ctx, i), 1);
        }

    public override Func<Context, int, IEnumerable<(string, IType)>> VisitNefieldtypes_nefieldtype([NotNull] FullSimpleParser.Nefieldtypes_nefieldtypeContext context)
    {
            var fieldType = _fieldTypesVisitor.Visit(context.fieldtype());
            var nefieldTypes = Visit(context.nefieldtypes());

            return (ctx, i) => Enumerable.Repeat(fieldType(ctx, i), 1).Concat(nefieldTypes(ctx, i + 1));
        }
}