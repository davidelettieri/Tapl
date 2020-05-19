using Common;

namespace FullSimple.Syntax.Types
{
    public class TypeVar : IType
    {
        public int X { get; }
        public int N { get; }

        public TypeVar(int x, int n)
        {
            X = x;
            N = n;
        }
    }
}
