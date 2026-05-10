using Common;

namespace FullRef.Syntax.Bindings;

public sealed class VarBind(IType type) : IBinding
{
    public IType Type { get; } = type;
}