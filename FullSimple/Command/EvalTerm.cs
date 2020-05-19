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

    public class TypeBinder : ICommand
    {
        public string Value { get; }
        public TypeBinder(string value)
        {
            Value = value;
        }
    }

    public class Binder : ICommand
    {
        public string Value { get; }
        public Binder(string value)
        {
            Value = value;
        }
    }
}
