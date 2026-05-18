using Common;

namespace FullPoly.Syntax.Terms;

public sealed class Float(IInfo info, double value) : ITerm
{
    public IInfo Info { get; } = info;
    public double Value { get; } = value;
}