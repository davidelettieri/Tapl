using Common;
using FullSimple.Syntax.Bindings;
using System;
using System.Linq;
using static FullSimple.Core.Shifting;

namespace FullSimple.Syntax;

public static class ContextExtensions
{
    public static IBinding GetBinding(this Context ctx, int i)
    {
            var b = ctx.Value.ElementAt(i).Item2;
            return BindingShift(i + 1, b);
        }

    public static IType GetTypeFromContext(this Context ctx, int i)
    {
            return GetBinding(ctx, i) switch
            {
                VarBind v => v.Type,
                TermAbbBind { Type: not null } t => t.Type,
                TermAbbBind => throw new Exception($"No type recorded for variable {ctx.IndexToName(i)}"),
                _ => throw new WrongKindOfBindException()
            };
        }
}