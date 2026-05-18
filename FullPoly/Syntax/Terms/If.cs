using Common;

namespace FullPoly.Syntax.Terms;

public sealed class If(IInfo info, ITerm condition, ITerm then, ITerm @else)
    : ITerm
{
    public IInfo Info { get; } = info;
    public ITerm Condition { get; } = condition;
    public ITerm Then { get; } = then;
    public ITerm Else { get; } = @else;
}