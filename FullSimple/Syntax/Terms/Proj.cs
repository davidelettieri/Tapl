using Common;

namespace FullSimple.Syntax.Terms;

public class Proj : ITerm
{
    public IInfo Info { get; }
    public ITerm Term { get; }
    public string Label { get; }

    public Proj(IInfo info, ITerm term, string s)
    {
            Info = info;
            Term = term;
            Label = s;
        }

    public override string ToString()
    {
            return $"TmProj({Term},{Label})";
        }
}