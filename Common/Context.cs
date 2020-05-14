using System;
using System.Collections.Immutable;
using System.Linq;

namespace Common
{
    public class Context
    {
        public ImmutableStack<(string, IBinding)> Value { get; }

        public Context()
        {
            Value = ImmutableStack<(string, IBinding)>.Empty;
        }

        public Context(ImmutableStack<(string, IBinding)> value)
        {
            Value = value;
        }
        public int Length => Value.Count();

        public Context AddBinding(string v, IBinding b) => new Context(Value.Push((v, b)));
        public Context AddName(string v) => AddBinding(v, new NameBinding());
        public bool IsNameBound(string x) => Value.Any(p => p.Item1 == x);
        public (Context, string) PickFreshName(string v)
        {
            if (IsNameBound(v))
                return PickFreshName(v + "'");

            return (AddName(v), v);
        }
        public string IndexToName(int i) => Value.ElementAt(i).Item1;
        public int NameToIndex(string v)
        {
            var count = 0;
            foreach (var item in Value)
            {
                if (item.Item1 == v)
                    return count;
                count++;
            }

            throw new Exception($"Identifier {v} is unbound");
        }
    }
}
