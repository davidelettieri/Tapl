using System.Collections.Generic;
using System.Linq;
using Common;

namespace FullRef.Syntax.Terms;

public sealed class Record(IInfo info, List<(string, ITerm)> fields) : ITerm
{
    public IInfo Info { get; } = info;
    public List<(string, ITerm)> Fields { get; } = fields;

    public override string ToString()
    {
        var fields = string.Join(",", Fields.Select(p => $"({p.Item1},{p.Item2})"));
        return $"TmRecord(List({fields}))";
    }
}