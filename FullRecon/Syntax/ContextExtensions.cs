using System;
using System.Linq;
using Common;
using FullRecon.Syntax.Bindings;

namespace FullRecon.Syntax;

public static class ContextExtensions
{
    extension(Context ctx)
    {
        public IBinding GetBinding(int i) => ctx.Value.ElementAt(i).Item2;

        public IType GetTypeFromContext(int i) =>
            ctx.GetBinding(i) switch
            {
                VarBind v => v.Type,
                _ => throw new WrongKindOfBindException()
            };
    }
}
