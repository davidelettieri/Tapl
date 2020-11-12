using Common;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text;

namespace FullSimple.Syntax.Terms
{
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
    }
}
