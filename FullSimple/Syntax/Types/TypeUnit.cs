using Common;

namespace FullSimple.Syntax.Types
{
    public class TypeUnit : IType
    {
        private TypeUnit() { }
        public static readonly TypeUnit Instance = new TypeUnit();
    }
}
