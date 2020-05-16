using Common;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text;

namespace FullSimple.Syntax.Terms
{

    public class Record : ITerm
    {
        public IInfo Info { get; }
        public ImmutableList<(string, ITerm)> Fields { get; }
        public Record(IInfo info, ImmutableList<(string, ITerm)> fields)
        {
            Info = info;
            Fields = fields;
        }
    }
}
