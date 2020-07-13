using Antlr4.Runtime.Misc;
using Common;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FullSimple.Visitors
{
    public class FieldTypesVisitor : FullSimpleBaseVisitor<Func<Context, int, IEnumerable<(string, IType)>>>
    {
        private static readonly NEFieldTypesVisitor _nefieldTypesVisitor = new NEFieldTypesVisitor();
        public override Func<Context, int, IEnumerable<(string, IType)>> VisitFieldtypes_nefieldtypes([NotNull] FullSimpleParser.Fieldtypes_nefieldtypesContext context)
        {
            if (context.nefieldtypes() is null)
                return (ctx, i) => Enumerable.Empty<(string, IType)>();

            var nefieldTypes = _nefieldTypesVisitor.Visit(context.nefieldtypes());

            return (ctx, i) => nefieldTypes(ctx, i);
        }
    }
}
