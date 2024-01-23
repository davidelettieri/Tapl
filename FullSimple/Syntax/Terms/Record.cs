using Common;
using System.Collections.Generic;
using System.Linq;

namespace FullSimple.Syntax.Terms;

public class Record : ITerm
{
    public IInfo Info { get; }
    public List<(string, ITerm)> Fields { get; }
    public Record(IInfo info, List<(string, ITerm)> fields)
    {
            Info = info;
            Fields = fields;
        }

    public override string ToString()
    {
            var fields = string.Join(",", Fields.Select(p => $"({p.Item1},{p.Item2})"));
            return $"TmRecord(List({fields}))";
        }
}