using Common;

namespace FullSimple.Syntax.Terms;

public class Let : ITerm
{
    public IInfo Info { get; }
    public string Variable { get; }
    public ITerm LetTerm { get; }
    public ITerm InTerm { get; }
    public Let(IInfo info, string variable, ITerm letTerm, ITerm inTerm)
    {
            Info = info;
            Variable = variable;
            LetTerm = letTerm;
            InTerm = inTerm;
        }
}