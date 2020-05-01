using Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace Chapter10.Syntax
{
    /// <summary>
    /// Lambda abstraction term λx.y
    /// </summary>
    public class Abs : ITerm
    {
        public ITerm Body { get; }
        public string BoundedVariable { get; }

        /// <summary>
        /// Lambda abstraction term λx.y
        /// </summary>
        /// <param name="body">The body of the lambda abstraction</param>
        /// <param name="bv">The bounded variable</param>
        public Abs(ITerm body, string bv)
        {
            Body = body;
            BoundedVariable = bv;
        }
    }
}
