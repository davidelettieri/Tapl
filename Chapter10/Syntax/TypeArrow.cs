using Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace Chapter10.Syntax
{
    public class TypeArrow<T, S> : IType where T : IType where S : IType
    {
    }

    public class TypeBool : IType { }


}
