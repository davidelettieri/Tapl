using Common;
using FullSimple.Syntax.Bindings;
using System;
using System.Linq;
using static FullSimple.Core.Shifting;

namespace FullSimple.Syntax
{
    public static class ContextExtensions
    {
        private static IBinding GetBinding(Context ctx, int i)
        {
            var b = ctx.Value.ElementAt(i).Item2;
            return BindingShift(i + 1, b);
        }

        public static IType GetTypeFromContext(this Context ctx, int i)
        {
            return GetBinding(ctx, i) switch
            {
                VarBind v => v.Type,
                TermAbbBind t when t.Type != null => t.Type,
                TermAbbBind _ => throw new Exception($"No type recorded for variable {ctx.IndexToName(i)}"),
                _ => throw new WrongKindOfBindException()
            };
        }
    }
}
