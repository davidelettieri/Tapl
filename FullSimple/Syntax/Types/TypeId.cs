using Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace FullSimple.Syntax.Types
{
    public class TypeId : IType
    {
        public string Name { get; }

        public TypeId(string name) => Name = name;
    }
}
