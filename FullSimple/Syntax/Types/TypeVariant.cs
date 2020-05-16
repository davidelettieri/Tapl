using Common;
using System.Collections.Immutable;

namespace FullSimple.Syntax.Types
{
    public class TypeVariant : IType
    {
        private ImmutableList<(string, IType)> Variants { get; }

        public TypeVariant(ImmutableList<(string, IType)> variants)
        {
            Variants = variants;
        }
    }
}
