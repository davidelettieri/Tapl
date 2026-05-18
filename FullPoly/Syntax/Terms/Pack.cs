using Common;

namespace FullPoly.Syntax.Terms;

public sealed class Pack(IInfo info, IType witnessType, ITerm term, IType existType) : ITerm
{
    public IInfo Info { get; } = info;
    public IType WitnessType { get; } = witnessType;
    public ITerm Term { get; } = term;
    public IType ExistType { get; } = existType;
}
