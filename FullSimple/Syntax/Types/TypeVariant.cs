using Common;
using System.Collections.Generic;

namespace FullSimple.Syntax.Types
{
    public class TypeVariant : IType
    {
        public IEnumerable<(string, IType)> Variants { get; }

        public TypeVariant(IEnumerable<(string, IType)> variants)
        {
            Variants = variants;
        }
    }
}
