using Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace SimpleBool.Syntax
{
    public class Var : ITerm
    {
        public IInfo Info { get; }
        public int Index { get; }
        public int ContextLength { get; }

        /// <summary>
        /// Variable term
        /// </summary>
        /// <param name="index">De bruijn index</param>
        /// <param name="ctxl">Context length</param>
        public Var(IInfo info, int index, int ctxl)
        {
            if (ctxl < index)
                throw new InvalidOperationException();

            Info = info;
            Index = index;
            ContextLength = ctxl;
        }
    }
}
