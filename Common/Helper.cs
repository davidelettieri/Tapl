using Antlr4.Runtime;
using System;
using System.Collections.Generic;
using System.Text;

namespace Common
{
    public static class Helper
    {
        public static IInfo GetFileInfo<T>(this T context) where T : ParserRuleContext
        {
            return new FileInfo(context.GetText(), context.Start.Line, context.Start.Column);
        }
    }
}
