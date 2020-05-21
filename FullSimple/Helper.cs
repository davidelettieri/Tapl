using Antlr4.Runtime;
using Antlr4.Runtime.Misc;
using FullSimple.Syntax;
using Common;
using System;
using System.Collections.Immutable;
using FullSimple.Syntax.Terms;
using FullSimple.Syntax.Types;

namespace FullSimple
{
    public static class Helper
    {
        public static IInfo GetFileInfo(ParserRuleContext context)
        {
            return new FileInfo(context.GetText(), context.Start.Line, context.Start.Column);
        }
    }
}
