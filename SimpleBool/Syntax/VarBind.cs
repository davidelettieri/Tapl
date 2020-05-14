using Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace Chapter10.Syntax
{
    public class VarBind : IBinding
    {
        public IType Type { get; }
        public VarBind(IType type)
        {
            Type = type;
        }
    }
}
