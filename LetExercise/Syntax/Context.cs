using Common;
using System.Linq;

namespace LetExercise.Syntax;

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