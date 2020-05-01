using Chapter10.Syntax;
using Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace Chapter10.Core
{
    public static class Evaluation
    {
        public static bool IsVal(Context ctx, ITerm t)
        {
            return t switch
            {
                True _ => true,
                False _ => true,
                Abs _ => true,
                _ => false
            };
        }

        private static ITerm Eval1(Context ctx, ITerm t)
        {
            return t switch
            {
                App app when IsVal(ctx,app.Right)
            }
        }
    }
}
