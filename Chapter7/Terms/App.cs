using Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace Chapter7.Terms
{
    /// <summary>
    /// Represents FX
    /// </summary>
    public class App : ITerm
    {
        private ITerm Left { get; }
        private ITerm Right { get; }

        public App(ITerm left, ITerm right)
        {
            Left = left;
            Right = right;
        }
    }
}
