using System;
using System.Linq;
using Common;
using FullError.Syntax.Bindings;
using static FullError.Core.Shifting;

namespace FullError.Syntax;

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
