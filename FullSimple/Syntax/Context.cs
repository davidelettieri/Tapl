using Common;
using FullSimple.Syntax.Bindings;
using System.Linq;

namespace FullSimple.Syntax
{
    public static class ContextExtensions
    {
        private static IBinding GetBinding(Context ctx, int i) => ctx.Value.ElementAt(i).Item2;
        public static IType GetTypeFromContext(this Context ctx, int i)
        {
            return GetBinding(ctx, i) switch
            {
                VarBind v => v.Type,
                _ => throw new WrongKindOfBindException()
            };
        }
    }
}
