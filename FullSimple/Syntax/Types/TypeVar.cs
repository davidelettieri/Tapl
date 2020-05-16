using Common;

namespace FullSimple.Syntax.Types
{
    public class TypeVar : IType
    {
        public int Int1 { get; }
        public int Int2 { get; }

        public TypeVar(int int1, int int2)
        {
            Int1 = int1;
            Int2 = int2;
        }
    }
}
