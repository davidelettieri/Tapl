using System;
using System.Collections.Generic;
using System.Linq;
using Common;
using FullRef.Syntax.Terms;

namespace FullRef.Core;

public sealed record Store(IReadOnlyList<ITerm> Terms)
{
    public static Store Empty { get; } = new Store(Array.Empty<ITerm>());

    public (int Location, Store Store) Extend(ITerm value)
    {
        var terms = Terms.ToList();
        terms.Add(value);
        return (Terms.Count, new Store(terms));
    }

    public ITerm Lookup(int location) => Terms[location];

    public Store Update(int location, ITerm value)
    {
        if (location < 0 || location >= Terms.Count)
            throw new InvalidOperationException("updatestore: bad index");

        var terms = Terms.ToList();
        terms[location] = value;
        return new Store(terms);
    }

    public Store Shift(int delta) => new Store(Terms.Select(t => Shifting.TermShift(delta, t)).ToList());
}
