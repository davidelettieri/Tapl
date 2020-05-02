using Common;
using System;
using System.Collections.Immutable;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Chapter10.Syntax
{
    public class Context
    {
        private readonly ImmutableStack<(string, IBinding)> _value;

        public Context()
        {
            _value = ImmutableStack<(string, IBinding)>.Empty;
        }

        internal IType GetTypeFromContext(Context ctx, int index)
        {
            throw new NotImplementedException();
        }

        public Context(ImmutableStack<(string, IBinding)> value)
        {
            _value = value;
        }

        public int Length => _value.Count();
        public Context AddBinding(string v, IBinding b) => new Context(_value.Push((v, b)));
        public Context AddName(string v) => AddBinding(v, new NameBinding());
        public bool IsNameBound(string x) => _value.Any(p => p.Item1 == x);
        public (Context, string) PickFreshName(string v)
        {
            if (IsNameBound(v))
                return PickFreshName(v + "'");

            return (AddName(v), v);
        }
        public string IndexToName(int i) => _value.ElementAt(i).Item1;

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

        private IBinding GetBinding(int i) => _value.ElementAt(i).Item2;

        public IType GetTypeFromContext(int i)
        {
            return GetBinding(i) switch
            {
                VarBind v => v.Type,
                _ => throw new WrongKindOfBindException()
            };
        }
    }
}
