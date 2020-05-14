using Common;

namespace Chapter10.Syntax
{
    public class True : ITerm
    {
        public IInfo Info { get; }
        public True(IInfo info)
        {
            Info = info;
        }
    }
}
