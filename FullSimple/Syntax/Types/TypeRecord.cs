using Common;
using System.Collections.Immutable;

namespace FullSimple.Syntax.Types
{
    public class TypeRecord : IType
    {
        private ImmutableList<(string, IType)> Variants { get; }

        public TypeRecord(ImmutableList<(string, IType)> variants)
        {
            Variants = variants;
        }
    }
}
