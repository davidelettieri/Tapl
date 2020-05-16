using Common;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text;

namespace FullSimple.Syntax.Terms
{
    public class Case : ITerm
    {
        public IInfo Info { get; }
        public ITerm Term { get; }
        public string S { get; }
        public List<(string, ITerm)> Cases { get; }

        public Case(IInfo info, ITerm term, string s, List<(string, ITerm)> cases)
        {
            Info = info;
            Term = term;
            S = s;
            Cases = cases;
        }
    }
}
