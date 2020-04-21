using Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace Chapter7.Terms
{
    /// <summary>
    /// Lambda abstraction term λx.y
    /// </summary>
    public class Abs : ITerm
    {
        public ITerm Body { get; }
        public string BoundedVariable { get; }
        public int ContextLength { get; }

        /// <summary>
        /// Lambda abstraction term λx.y
        /// </summary>
        /// <param name="body">The body of the lambda abstraction</param>
        /// <param name="bv">The bounded variable</param>
        /// <param name="ctxl">Context length</param>
        public Abs(ITerm body, string bv, int ctxl)
        {
            Body = body;
            ContextLength = ctxl;
            BoundedVariable = bv;
        }
    }
}
