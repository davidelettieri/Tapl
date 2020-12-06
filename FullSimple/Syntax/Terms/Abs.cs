using Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace FullSimple.Syntax.Terms
{
    /// <summary>
    /// Lambda abstraction term λx.y
    /// </summary>
    public class Abs : ITerm
    {
        public IInfo Info { get; }
        public ITerm Body { get; }
        public string V { get; }
        public IType Type { get; }

        /// <summary>
        /// Lambda abstraction term λx.y
        /// </summary>
        /// <param name="body">The body of the lambda abstraction</param>
        /// <param name="v">The bounded variable</param>
        public Abs(IInfo info, string v, IType type, ITerm body)
        {
            Info = info;
            V = v;
            Type = type;
            Body = body;
        }

        public override string ToString()
        {
            return $"TmAbs({V},{Type},{Body})";
        }
    }
}
