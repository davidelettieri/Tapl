using Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace Chapter4.Terms
{
    public class IsZero : ITerm
    {
        public ITerm Term { get; }

        public IsZero(ITerm term)
        {
            Term = term;
        }
    }
}
