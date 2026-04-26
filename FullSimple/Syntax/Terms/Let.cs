using Common;

namespace FullSimple.Syntax.Terms;

public sealed class Let(IInfo info, string variable, ITerm letTerm, ITerm inTerm)
    : ITerm
{
    public IInfo Info { get; } = info;
    public string Variable { get; } = variable;
    public ITerm LetTerm { get; } = letTerm;
    public ITerm InTerm { get; } = inTerm;
}