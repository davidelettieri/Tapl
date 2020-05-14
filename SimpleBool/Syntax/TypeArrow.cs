using Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace SimpleBool.Syntax
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
