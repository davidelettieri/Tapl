using Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace Chapter7.Terms
{
    public class Var : ITerm
    {
        public int Index { get; }
        public int ContextLength { get; }

        /// <summary>
        /// Variable term
        /// </summary>
        /// <param name="index">De bruijn index</param>
        /// <param name="ctxl">Context length</param>
        public Var(int index, int ctxl)
        {
            Index = index;

            if (ctxl < index)
                throw new InvalidOperationException();

            ContextLength = ctxl;
        }
    }
}
