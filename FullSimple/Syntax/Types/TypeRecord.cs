using Common;
using System.Collections.Generic;

namespace FullSimple.Syntax.Types
{
    public class TypeRecord : IType
    {
        public IEnumerable<(string, IType)> Variants { get; }

        public TypeRecord(IEnumerable<(string, IType)> variants)
        {
            Variants = variants;
        }
    }
}
