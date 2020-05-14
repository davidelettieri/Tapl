using Common;

namespace Chapter10.Syntax
{
    public class False : ITerm
    {
        public IInfo Info { get; }
        public False(IInfo info)
        {
            Info = info;
        }
    }
}
