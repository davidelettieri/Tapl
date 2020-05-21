using Antlr4.Runtime;
using Antlr4.Runtime.Misc;
using FullSimple.Syntax;
using Common;
using System;
using System.Collections.Immutable;
using FullSimple.Syntax.Terms;
using FullSimple.Syntax.Types;
using static FullSimple.Helper;
using FullSimple.Command;
using System.Collections;
using System.Collections.Generic;

namespace FullSimple.Visitors
{
    public class CasesVisitor : FullSimpleBaseVisitor<Func<Context, IEnumerable<(string, string, ITerm)>>>
    {

    }
}
