using Common;
using System.Collections.Generic;

namespace FullSimple.Syntax.Terms
{
    public class Record : ITerm
    {
        public IInfo Info { get; }
        public List<(string, ITerm)> Fields { get; }
        public Record(IInfo info, List<(string, ITerm)> fields)
        {
            Info = info;
            Fields = fields;
        }
    }
}
