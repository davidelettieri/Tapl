using System;
using System.Collections.Immutable;
using System.Linq;

namespace Chapter7
{
    public class Context
    {
        private readonly ImmutableStack<(string, Binding)> _value;

        public Context()
        {
            _value = ImmutableStack<(string, Binding)>.Empty;
        }

        public Context(ImmutableStack<(string, Binding)> value)
        {
            _value = value;
        }

        public int Length => _value.Count();
        public Context AddBinding(string v, Binding b) => new Context(_value.Push((v, b)));
        public Context AddName(string v) => AddBinding(v, new Binding());
        public bool IsNameBound(string x) => _value.Any(p => p.Item1 == x);
        public (Context, string) PickFreshName(string v)
        {
            if (IsNameBound(v))
                return PickFreshName(v + "'");

            return (AddName(v), v);
        }

        public string IndexToName(int idx)
        {
            var count = 0;

            foreach (var item in _value)
            {
                if (count == idx)
                    return item.Item1;
                count++;
            }

            throw new Exception();
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
