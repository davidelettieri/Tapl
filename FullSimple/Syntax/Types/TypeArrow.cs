using Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace FullSimple.Syntax.Types
{
    public class TypeArrow : IType
    {
        public IType From { get; }
        public IType To { get; }

        public TypeArrow(IType from, IType to)
        {
            From = from;
            To = to;
        }
    }
}
