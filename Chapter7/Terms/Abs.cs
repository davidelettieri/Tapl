using Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace Chapter7.Terms
{
    public class Abs : ITerm
    {
        public ITerm Body { get; }

        public Abs(ITerm body)
        {
            Body = body;
        }
    }
}
