using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;

namespace Chapter7
{
    public class Context
    {
        private readonly ImmutableList<(string, Binding)> _value;

        public Context()
        {
            _value = ImmutableList<(string, Binding)>.Empty;
        }

        public Context(ImmutableList<(string, Binding)> value)
        {
            _value = value;
        }

        public Context Add(string v, Binding b) => new Context(_value.Add((v, b)));

        public int Length => _value.Count;
        public bool IsFresh(string v) => !_value.Select(p => p.Item1).Contains(v);

        public string IndexToName(int idx)
        {
            if (idx > _value.Count)
                throw new InvalidOperationException();

            return _value[idx].Item1;
        }
    }
}
