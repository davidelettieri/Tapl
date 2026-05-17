using System;
using System.Linq;
using Common;
using FullPoly.Syntax.Bindings;
using static FullPoly.Core.Shifting;

namespace FullPoly.Syntax;

public static class ContextExtensions
{
    extension(Context ctx)
    {
        public IBinding GetBinding(int i)
        {
            var b = ctx.Value.ElementAt(i).Item2;
            return BindingShift(i + 1, b);
        }

        public IType GetTypeFromContext(int i) =>
            ctx.GetBinding(i) switch
            {
                VarBind v => v.Type,
                TermAbbBind { Type: not null } t => t.Type,
                TermAbbBind => throw new Exception($"No type recorded for variable {ctx.IndexToName(i)}"),
                _ => throw new WrongKindOfBindException()
            };
    }
}