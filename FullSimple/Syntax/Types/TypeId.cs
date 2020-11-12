using Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace FullSimple.Syntax.Types
{
    public class TypeId : IType, IEquatable<TypeId>
    {
        public string Name { get; }

        public TypeId(string name) => Name = name;

        public override bool Equals(object obj)
        {
            return obj is TypeId id && Equals(id);
        }

        public bool Equals(TypeId obj) => Name == obj?.Name;

        public override int GetHashCode()
        {
            return HashCode.Combine(Name);
        }
    }
}
