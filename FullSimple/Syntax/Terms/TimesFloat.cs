using Common;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text;

namespace FullSimple.Syntax.Terms
{

    public class TimesFloat : ITerm
    {
        public IInfo Info { get; }
        public ITerm Left { get; }
        public ITerm Right { get; }

        public TimesFloat(IInfo info, ITerm left, ITerm right)
        {
            Info = info;
            Left = left;
            Right = right;
        }
    }
}
