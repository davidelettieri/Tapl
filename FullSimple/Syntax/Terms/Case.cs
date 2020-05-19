using Common;
using System.Collections.Generic;

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
    }
}
