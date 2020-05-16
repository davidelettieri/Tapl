using Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace FullSimple.Syntax.Terms
{
    public class Fix : ITerm
    {
        public IInfo Info { get; }
        public ITerm Term { get; }
        public Fix(IInfo info, ITerm term)
        {
            Info = info;
            Term = term;
        }
    }
}
