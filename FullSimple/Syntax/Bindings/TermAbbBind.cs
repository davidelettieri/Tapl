using Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace FullSimple.Syntax.Bindings
{
    public class TermAbbBind : IBinding
    {
        public ITerm Term { get; }
        public IType Type { get; }
        public TermAbbBind(ITerm term, IType type)
        {
            Term = term;
            Type = type;
        }
    }
}
