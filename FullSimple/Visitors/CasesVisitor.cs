using Common;
using System;
using System.Collections.Generic;

namespace FullSimple.Visitors
{
    public class CasesVisitor : FullSimpleBaseVisitor<Func<Context, IEnumerable<(string, string, ITerm)>>>
    {

    }
}
