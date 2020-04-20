using Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace Chapter7.Terms
{
    public class Var : ITerm
    {
        public int Index { get; }

        public Var(int index)
        {
            Index = index;
        }
    }
}
