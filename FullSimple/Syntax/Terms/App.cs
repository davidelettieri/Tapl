using Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace FullSimple.Syntax.Terms
{
    /// <summary>
    /// Application term (xy)
    /// </summary>
    public class App : ITerm
    {
        public IInfo Info { get; }
        public ITerm Left { get; }
        public ITerm Right { get; }

        /// <summary>
        /// Application term (xy)
        /// </summary>
        /// <param name="left">The first term in the application</param>
        /// <param name="right">The second term in the application</param>
        public App(IInfo info, ITerm left, ITerm right)
        {
            Info = info;
            Left = left;
            Right = right;
        }
    }
}
