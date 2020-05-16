using Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace FullSimple.Syntax.Terms
{

    public class String : ITerm
    {
        public string Value { get; }
        public String(string value)
        {
            Value = value;
        }
    }
}
