using Common;
using System.Collections.Generic;
using System.Linq;

namespace FullSimple.Syntax.Terms
{
    public class Case : ITerm
    {
        public IInfo Info { get; }
        public ITerm Term { get; }
        public IEnumerable<(string label, string variable, ITerm term)> Cases { get; }

        public Case(IInfo info, ITerm term, IEnumerable<(string, string, ITerm)> cases)
        {
            Info = info;
            Term = term;
            Cases = cases;
        }

        public override string ToString()
        {
            var cases = string.Join(",", Cases.Select(p => $"({p.label},{p.variable},{p.term})"));
            return $"TmCase({Term},List({cases}))";
        }
    }
}
