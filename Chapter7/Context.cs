using System;
using System.Collections.Immutable;
using System.Linq;

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
        public bool IsFresh(string x) => !_value.Select(p => p.Item1).Contains(x);
        public bool IsNameBound(string x) => !IsFresh(x);
        public (Context, string) PickFreshName(string v)
        {
            if (IsFresh(v))
                return (Add(v, new Binding()), v);

            return PickFreshName(v + "'");
        }
        public string IndexToName(int idx)
        {
            if (idx > _value.Count)
                throw new InvalidOperationException();

            return _value[idx].Item1;
        }
        public int NameToIndex(string v)
        {
            var count = 0;
            foreach (var item in _value)
            {
                if (item.Item1 == v)
                    return count;
                count++;
            }

            throw new Exception($"Identifier {v} is unbound");
        }
    }
}
