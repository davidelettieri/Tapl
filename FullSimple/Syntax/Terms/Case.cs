using Common;
using System.Collections.Generic;
using System.Linq;

namespace FullSimple.Syntax.Terms;

public sealed class Case(IInfo info, ITerm term, IEnumerable<(string, string, ITerm)> cases)
    : ITerm
{
    public IInfo Info { get; } = info;
    public ITerm Term { get; } = term;
    public IEnumerable<(string label, string variable, ITerm term)> Cases { get; } = cases;

    public override string ToString()
    {
        var cases = string.Join(",", Cases.Select(p => $"({p.label},{p.variable},{p.term})"));
        return $"TmCase({Term},List({cases}))";
    }
}