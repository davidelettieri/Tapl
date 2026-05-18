using Common;

namespace FullRecon.Syntax.Terms;

/// <summary>
/// Lambda abstraction. Type may be null when the type is to be inferred.
/// </summary>
public class Abs(IInfo info, string v, IType? type, ITerm body) : ITerm
{
    public IInfo Info { get; } = info;
    public string V { get; } = v;
    public IType? Type { get; } = type;
    public ITerm Body { get; } = body;
}
