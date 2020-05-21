using Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace FullSimple.Syntax.Bindings
{
    public class TypeAbbBind : IBinding
    {
        public IType Type { get; }
        public TypeAbbBind(IType type)
        {
            Type = type;
        }
    }
}
