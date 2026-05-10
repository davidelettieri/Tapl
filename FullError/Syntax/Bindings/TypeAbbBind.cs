using Common;

namespace FullError.Syntax.Bindings;

public sealed class TypeAbbBind(IType type) : IBinding
{
    public IType Type { get; } = type;
}
