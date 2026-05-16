using Common;
using FullUpdate.Syntax;

namespace FullUpdate.Syntax.Bindings;

public sealed class VarBind(IType type) : IBinding { public IType Type { get; } = type; }
public sealed class TypeVarBind(IType bound) : IBinding { public IType Bound { get; } = bound; }
public sealed class TypeAbbBind(IType type, IKind? kind) : IBinding
{
    public IType Type { get; } = type;
    public IKind? Kind { get; } = kind;
}
public sealed class TermAbbBind(ITerm term, IType? type) : IBinding
{
    public ITerm Term { get; } = term;
    public IType? Type { get; } = type;
}
