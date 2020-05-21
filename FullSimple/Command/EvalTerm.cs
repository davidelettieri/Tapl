using Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace FullSimple.Command
{
    public class EvalTerm : ICommand
    {
        public ITerm Term { get; }

        public EvalTerm(ITerm term)
        {
            Term = term;
        }
    }

    public class Binder : ICommand
    {
        public string Id { get; }
        public IBinding Binding { get; }

        public Binder(string id, IBinding binding)
        {
            Id = id;
            Binding = binding;
        }
    }
}
