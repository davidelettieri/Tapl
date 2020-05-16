using Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace LetExercise.Syntax
{
    public class VarBind : IBinding
    {
        public IType Type { get; }
        public VarBind(IType type)
        {
            Type = type;
        }
    }
}
