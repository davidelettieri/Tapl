using Common;

namespace FullUntyped.Syntax.Bindings;

public sealed class TermAbbBind(ITerm term, IType? type) : IBinding
{
    public ITerm Term { get; } = term;
    public IType? Type { get; } = type;

    public override string ToString() => $"TmAbbBind({Term},{Type?.ToString() ?? "None"})";
}