using Common;

namespace FullRecon.Syntax.Bindings;

public sealed class VarBind(IType type) : IBinding
{
    public IType Type { get; } = type;
}
